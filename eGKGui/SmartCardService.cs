namespace eGKGui;

internal sealed class SmartCardService : IDisposable
{
    private nint _context = nint.Zero;
    private nint _card = nint.Zero;
    private uint _activeProtocol = PCSCInterop.SCARD_PROTOCOL_UNDEFINED;

    public bool IsConnected => _card != nint.Zero;
    public uint ActiveProtocol => _activeProtocol;

    public IReadOnlyList<string> ListReaders()
    {
        EnsureContext();

        uint cchReaders = 0;
        uint result = PCSCInterop.SCardListReaders(_context, null, null, ref cchReaders);

        if (result == PCSCInterop.SCARD_E_NO_READERS_AVAILABLE || cchReaders == 0)
            return [];
        if (result != PCSCInterop.SCARD_S_SUCCESS)
            throw new SmartCardException($"SCardListReaders failed: 0x{result:X8}");

        char[] buffer = new char[cchReaders];
        result = PCSCInterop.SCardListReaders(_context, null, buffer, ref cchReaders);
        if (result != PCSCInterop.SCARD_S_SUCCESS)
            throw new SmartCardException($"SCardListReaders failed: 0x{result:X8}");

        return ParseMultiString(buffer);
    }

    public void Connect(string readerName)
    {
        EnsureContext();
        if (_card != nint.Zero) Disconnect();

        uint result = PCSCInterop.SCardConnect(
            _context, readerName,
            PCSCInterop.SCARD_SHARE_SHARED,
            PCSCInterop.SCARD_PROTOCOL_ANY,
            out _card,
            out _activeProtocol);

        if (result != PCSCInterop.SCARD_S_SUCCESS)
            throw new SmartCardException($"SCardConnect failed: 0x{result:X8}");
    }

    public void Disconnect()
    {
        if (_card == nint.Zero) return;
        PCSCInterop.SCardDisconnect(_card, PCSCInterop.SCARD_LEAVE_CARD);
        _card = nint.Zero;
        _activeProtocol = PCSCInterop.SCARD_PROTOCOL_UNDEFINED;
    }

    public byte[] Transmit(byte[] apdu)
    {
        if (_card == nint.Zero)
            throw new InvalidOperationException("Not connected to a card.");

        var pci = PCSCInterop.SCARD_IO_REQUEST.ForProtocol(
            _activeProtocol == PCSCInterop.SCARD_PROTOCOL_T0
                ? PCSCInterop.SCARD_PROTOCOL_T0
                : PCSCInterop.SCARD_PROTOCOL_T1);

        byte[] recvBuffer = new byte[65538]; // supports extended APDUs + SW
        uint recvLen = (uint)recvBuffer.Length;

        uint result = PCSCInterop.SCardTransmit(
            _card, ref pci,
            apdu, (uint)apdu.Length,
            nint.Zero,
            recvBuffer, ref recvLen);

        if (result != PCSCInterop.SCARD_S_SUCCESS)
            throw new SmartCardException($"SCardTransmit failed: 0x{result:X8}");

        return recvBuffer[..(int)recvLen];
    }

    public (string Atr, uint State, uint Protocol) GetStatus()
    {
        if (_card == nint.Zero)
            throw new InvalidOperationException("Not connected to a card.");

        uint readerLen = 0;
        uint atrLen = 32;
        byte[] atr = new byte[atrLen];

        uint result = PCSCInterop.SCardStatus(
            _card, nint.Zero, ref readerLen,
            out uint state, out uint protocol,
            atr, ref atrLen);

        if (result != PCSCInterop.SCARD_S_SUCCESS)
            throw new SmartCardException($"SCardStatus failed: 0x{result:X8}");

        string atrHex = atrLen > 0
            ? BitConverter.ToString(atr[..(int)atrLen]).Replace("-", " ")
            : "(empty)";

        return (atrHex, state, protocol);
    }

    private void EnsureContext()
    {
        if (_context != nint.Zero) return;

        uint result = PCSCInterop.SCardEstablishContext(
            PCSCInterop.SCARD_SCOPE_USER,
            nint.Zero, nint.Zero,
            out _context);

        if (result != PCSCInterop.SCARD_S_SUCCESS)
            throw new SmartCardException($"SCardEstablishContext failed: 0x{result:X8}");
    }

    private static List<string> ParseMultiString(char[] buffer)
    {
        var readers = new List<string>();
        int start = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            if (buffer[i] == '\0')
            {
                if (i > start)
                    readers.Add(new string(buffer, start, i - start));
                else
                    break; // double-null terminator
                start = i + 1;
            }
        }
        return readers;
    }

    public void Dispose()
    {
        Disconnect();
        if (_context != nint.Zero)
        {
            PCSCInterop.SCardReleaseContext(_context);
            _context = nint.Zero;
        }
    }
}
