using System.Runtime.InteropServices;

namespace eGKGui;

internal static class PCSCInterop
{
    // Scope
    public const uint SCARD_SCOPE_USER = 0;

    // Share mode
    public const uint SCARD_SHARE_SHARED = 2;
    public const uint SCARD_SHARE_EXCLUSIVE = 1;

    // Protocol
    public const uint SCARD_PROTOCOL_UNDEFINED = 0;
    public const uint SCARD_PROTOCOL_T0 = 1;
    public const uint SCARD_PROTOCOL_T1 = 2;
    public const uint SCARD_PROTOCOL_ANY = 3;

    // Disposition on disconnect
    public const uint SCARD_LEAVE_CARD = 0;
    public const uint SCARD_RESET_CARD = 1;
    public const uint SCARD_UNPOWER_CARD = 2;

    // Return codes
    public const uint SCARD_S_SUCCESS = 0;
    public const uint SCARD_E_NO_READERS_AVAILABLE = 0x8010002E;

    [StructLayout(LayoutKind.Sequential)]
    public struct SCARD_IO_REQUEST
    {
        public uint dwProtocol;
        public uint cbPciLength;

        public static SCARD_IO_REQUEST ForProtocol(uint protocol) => new()
        {
            dwProtocol = protocol,
            cbPciLength = 8  // sizeof(SCARD_IO_REQUEST)
        };
    }

    [DllImport("winscard.dll", CharSet = CharSet.Unicode, EntryPoint = "SCardEstablishContext")]
    public static extern uint SCardEstablishContext(
        uint dwScope,
        nint pvReserved1,
        nint pvReserved2,
        out nint phContext);

    [DllImport("winscard.dll", EntryPoint = "SCardReleaseContext")]
    public static extern uint SCardReleaseContext(nint hContext);

    [DllImport("winscard.dll", CharSet = CharSet.Unicode, EntryPoint = "SCardListReadersW")]
    public static extern uint SCardListReaders(
        nint hContext,
        string? mszGroups,
        char[]? mszReaders,
        ref uint pcchReaders);

    [DllImport("winscard.dll", CharSet = CharSet.Unicode, EntryPoint = "SCardConnectW")]
    public static extern uint SCardConnect(
        nint hContext,
        string szReader,
        uint dwShareMode,
        uint dwPreferredProtocols,
        out nint phCard,
        out uint pdwActiveProtocol);

    [DllImport("winscard.dll", EntryPoint = "SCardDisconnect")]
    public static extern uint SCardDisconnect(nint hCard, uint dwDisposition);

    [DllImport("winscard.dll", EntryPoint = "SCardTransmit")]
    public static extern uint SCardTransmit(
        nint hCard,
        ref SCARD_IO_REQUEST pioSendPci,
        byte[] pbSendBuffer,
        uint cbSendLength,
        nint pioRecvPci,
        byte[] pbRecvBuffer,
        ref uint pcbRecvLength);

    // mszReaderNames passed as nint so callers can pass nint.Zero without marshaling overhead
    [DllImport("winscard.dll", CharSet = CharSet.Unicode, EntryPoint = "SCardStatusW")]
    public static extern uint SCardStatus(
        nint hCard,
        nint mszReaderNames,
        ref uint pcchReaderLen,
        out uint pdwState,
        out uint pdwProtocol,
        byte[] pbAtr,
        ref uint pcbAtrLen);
}
