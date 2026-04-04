using PCSC;

namespace eGKGui;

public partial class Form1 : Form
{
    private readonly SmartCardService _smartCardService = new();
    private EgkHttpServer? _httpServer;

    public Form1()
    {
        InitializeComponent();
        Load += Form1_Load;
    }

    // ── Lifecycle ──────────────────────────────────────────────────────────

    private void Form1_Load(object? sender, EventArgs e)
    {
        RefreshReaders();
        BtnStartServer_Click(sender, e);
    }

    // ── Reader management ─────────────────────────────────────────────────

    private void BtnRefresh_Click(object? sender, EventArgs e) => RefreshReaders();

    private void RefreshReaders()
    {
        try
        {
            var readers = _smartCardService.ListReaders();
            string? current = cmbReaders.SelectedItem as string;
            cmbReaders.Items.Clear();
            foreach (var r in readers)
                cmbReaders.Items.Add(r);

            // Restore previous selection if still present, otherwise pick first
            int idx = current is not null ? cmbReaders.Items.IndexOf(current) : -1;
            cmbReaders.SelectedIndex = idx >= 0 ? idx : (cmbReaders.Items.Count > 0 ? 0 : -1);

            Log($"Found {readers.Count} reader(s).");
        }
        catch (SmartCardException ex)
        {
            Log($"Error listing readers: {ex.Message}", LogKind.Error);
        }
    }

    private void BtnConnect_Click(object? sender, EventArgs e)
    {
        if (cmbReaders.SelectedItem is not string readerName) return;
        try
        {
            _smartCardService.Connect(readerName);
            var (atr, _, protocol) = _smartCardService.GetStatus();

            lblStatusValue.Text = "Connected";
            lblStatusValue.ForeColor = Color.Green;
            lblAtrValue.Text = atr;
            lblProtocolValue.Text = protocol switch
            {
                PCSCInterop.SCARD_PROTOCOL_T0 => "T=0",
                PCSCInterop.SCARD_PROTOCOL_T1 => "T=1",
                _ => $"0x{protocol:X}"
            };

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            btnSend.Enabled = true;

            Log($"Connected to: {readerName}");
            Log($"ATR:      {atr}");
            Log($"Protocol: {lblProtocolValue.Text}");
        }
        catch (SmartCardException ex)
        {
            Log($"Connection failed: {ex.Message}", LogKind.Error);
        }
    }

    private void BtnDisconnect_Click(object? sender, EventArgs e)
    {
        _smartCardService.Disconnect();
        lblStatusValue.Text = "Not connected";
        lblStatusValue.ForeColor = Color.Gray;
        lblAtrValue.Text = "—";
        lblProtocolValue.Text = "—";

        btnConnect.Enabled = true;
        btnDisconnect.Enabled = false;
        btnSend.Enabled = false;

        Log("Disconnected.");
    }



    // ── HTTP Server ───────────────────────────────────────────────────────

    private void BtnStartServer_Click(object? sender, EventArgs e)
    {
        try
        {
            _httpServer = new EgkHttpServer();
            _httpServer.Start();
            btnStartServer.Enabled = false;
            btnStopServer.Enabled = true;
            lblServerStatus.Text = "Running on http://localhost:5000/";
            lblServerStatus.ForeColor = Color.Green;
            Log("HTTP server started on http://localhost:5000/egk");
        }
        catch (Exception ex)
        {
            Log($"Failed to start HTTP server: {ex.Message}", LogKind.Error);
        }
    }

    private void BtnStopServer_Click(object? sender, EventArgs e)
    {
        _httpServer?.Dispose();
        _httpServer = null;
        btnStartServer.Enabled = true;
        btnStopServer.Enabled = false;
        lblServerStatus.Text = "Stopped";
        lblServerStatus.ForeColor = Color.Gray;
        Log("HTTP server stopped.");
    }

    private void BtnReadeGK(object? sender, EventArgs e)
    {
        Log($"Reading eGK",LogKind.Normal);
        using var ctx = ContextFactory.Instance.Establish(SCardScope.System);
        string[] readerNames = ctx.GetReaders();
        if (readerNames.Length == 0) throw new Exception("No reader found.");
        Log($"Found {readerNames.Length} readers");
        
        if (cmbReaders.SelectedItem is not string readerName)
        {
            Log("No Reader selected");
            return;
        }
        Log($"Selecting: {readerName}");
        using var reader = ctx.ConnectReader(readerName, SCardShareMode.Shared, SCardProtocol.Any);
        var egk = new EgkReader(reader);
        HealthCardData data = egk.GetData();

       Log($"Name: {data.FirstName} {data.LastName}");
       Log($"Born: {data.Birthdate}");

    }

    // ── Clear log ─────────────────────────────────────────────────────────

    private void BtnClear_Click(object? sender, EventArgs e) => rtbResponse.Clear();

    // ── Logging ───────────────────────────────────────────────────────────

    private enum LogKind { Normal, Success, Warning, Error }

    private static readonly Dictionary<LogKind, Color> LogColors = new()
    {
        [LogKind.Normal]  = Color.FromArgb(180, 255, 180),
        [LogKind.Success] = Color.FromArgb(100, 220, 100),
        [LogKind.Warning] = Color.FromArgb(255, 200, 60),
        [LogKind.Error]   = Color.FromArgb(255, 90, 90),
    };

    private void Log(string message, LogKind kind = LogKind.Normal)
    {
        string ts = DateTime.Now.ToString("HH:mm:ss.fff");
        rtbResponse.SelectionStart = rtbResponse.TextLength;
        rtbResponse.SelectionLength = 0;
        rtbResponse.SelectionColor = LogColors[kind];
        rtbResponse.AppendText($"[{ts}] {message}{Environment.NewLine}");
        rtbResponse.ScrollToCaret();
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static byte[]? ParseHex(string hex)
    {
        hex = hex.Replace(" ", "").Replace("-", "").Replace(":", "");
        if (hex.Length == 0 || hex.Length % 2 != 0) return null;
        try
        {
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return bytes;
        }
        catch { return null; }
    }

    private static string FormatHex(byte[] bytes) =>
        bytes.Length == 0 ? "(empty)" : BitConverter.ToString(bytes).Replace("-", " ");

    private static string GetSwDescription(byte sw1, byte sw2) => (sw1, sw2) switch
    {
        (0x90, 0x00) => "Success",
        (0x61, _)    => $"Response bytes available: {sw2}",
        (0x62, 0x81) => "Part of returned data may be corrupted",
        (0x62, 0x82) => "End of file reached before reading Le bytes",
        (0x63, 0x00) => "Verification failed",
        (0x63, _) when sw2 is >= 0xC0 and <= 0xCF => $"Verification failed, {sw2 & 0x0F} tries remaining",
        (0x64, _)    => "State unchanged (error)",
        (0x65, 0x81) => "Memory failure",
        (0x67, 0x00) => "Wrong length (Le/Lc)",
        (0x68, 0x81) => "Logical channel not supported",
        (0x68, 0x82) => "Secure messaging not supported",
        (0x69, 0x81) => "Command incompatible with file structure",
        (0x69, 0x82) => "Security status not satisfied",
        (0x69, 0x83) => "Authentication method blocked",
        (0x69, 0x84) => "Referenced data invalid",
        (0x69, 0x85) => "Conditions of use not satisfied",
        (0x69, 0x86) => "Command not allowed (no current EF)",
        (0x69, 0x87) => "Expected secure messaging objects missing",
        (0x69, 0x88) => "Incorrect secure messaging data objects",
        (0x6A, 0x80) => "Incorrect data in command data field",
        (0x6A, 0x81) => "Function not supported",
        (0x6A, 0x82) => "File or application not found",
        (0x6A, 0x83) => "Record not found",
        (0x6A, 0x84) => "Not enough memory space in file",
        (0x6A, 0x85) => "Lc inconsistent with TLV structure",
        (0x6A, 0x86) => "Incorrect P1/P2",
        (0x6A, 0x87) => "Lc inconsistent with P1/P2",
        (0x6A, 0x88) => "Referenced data not found",
        (0x6B, _)    => "Wrong P1/P2",
        (0x6C, _)    => $"Wrong Le — correct Le is {sw2:X2}",
        (0x6D, _)    => "Instruction code not supported",
        (0x6E, _)    => "Class not supported",
        (0x6F, _)    => "Unknown / unspecified error",
        _            => string.Empty
    };
}
