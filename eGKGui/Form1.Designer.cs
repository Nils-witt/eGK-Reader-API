namespace eGKGui
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _smartCardService?.Dispose();
                _httpServer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            grpReader = new GroupBox();
            lblReader = new Label();
            cmbReaders = new ComboBox();
            btnRefresh = new Button();
            btnConnect = new Button();
            btnDisconnect = new Button();

            grpStatus = new GroupBox();
            lblStatusCaption = new Label();
            lblStatusValue = new Label();
            lblAtrCaption = new Label();
            lblAtrValue = new Label();
            lblProtocolCaption = new Label();
            lblProtocolValue = new Label();

            grpCommand = new GroupBox();
            lblApduCaption = new Label();
            txtApdu = new TextBox();
            btnSend = new Button();
            btnSelectMF = new Button();
            btnGetChallenge = new Button();
            btnSelectEfGdo = new Button();
            btnReadBinary = new Button();
            btnGetResponse = new Button();

            grpServer = new GroupBox();
            btnStartServer = new Button();
            btnStopServer = new Button();
            lblServerStatus = new Label();

            grpResponse = new GroupBox();
            rtbResponse = new RichTextBox();
            btnClear = new Button();

            // ── GroupBox: Reader ──────────────────────────────────────────
            grpReader.Text = "Reader";
            grpReader.Location = new Point(8, 8);
            grpReader.Size = new Size(904, 58);
            grpReader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblReader.Text = "Reader:";
            lblReader.Location = new Point(8, 24);
            lblReader.Size = new Size(50, 23);
            lblReader.TextAlign = ContentAlignment.MiddleLeft;
            lblReader.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            cmbReaders.Location = new Point(62, 22);
            cmbReaders.Size = new Size(534, 23);
            cmbReaders.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReaders.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnRefresh.Text = "Refresh";
            btnRefresh.Location = new Point(604, 21);
            btnRefresh.Size = new Size(75, 25);
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.Click += BtnRefresh_Click;

            grpReader.Controls.AddRange([lblReader, cmbReaders, btnRefresh]);

          

            // ── GroupBox: APDU Command ────────────────────────────────────
            grpCommand.Text = "Debug";
            grpCommand.Location = new Point(8, 74);
            grpCommand.Size = new Size(904, 60);
            grpCommand.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnSelectEfGdo.Text = "Test read eGK";
            btnSelectEfGdo.Location = new Point(8, 25);
            btnSelectEfGdo.Size = new Size(300, 25);
            btnSelectEfGdo.Click += BtnReadeGK;


            grpCommand.Controls.Add(btnSelectEfGdo);

            // ── GroupBox: HTTP Server ─────────────────────────────────────
            grpServer.Text = "HTTP Server";
            grpServer.Location = new Point(8, 142);
            grpServer.Size = new Size(904, 56);
            grpServer.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnStartServer.Text = "Start Server";
            btnStartServer.Location = new Point(8, 21);
            btnStartServer.Size = new Size(100, 25);
            btnStartServer.Click += BtnStartServer_Click;

            btnStopServer.Text = "Stop Server";
            btnStopServer.Location = new Point(116, 21);
            btnStopServer.Size = new Size(100, 25);
            btnStopServer.Enabled = false;
            btnStopServer.Click += BtnStopServer_Click;

            lblServerStatus.Text = "Stopped";
            lblServerStatus.Location = new Point(228, 21);
            lblServerStatus.Size = new Size(300, 25);
            lblServerStatus.ForeColor = Color.Gray;
            lblServerStatus.TextAlign = ContentAlignment.MiddleLeft;

            grpServer.Controls.AddRange([btnStartServer, btnStopServer, lblServerStatus]);

            // ── GroupBox: Response Log ────────────────────────────────────
            grpResponse.Text = "Response Log";
            grpResponse.Location = new Point(8, 206);
            grpResponse.Size = new Size(904, 400);
            grpResponse.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            rtbResponse.Location = new Point(8, 22);
            rtbResponse.Size = new Size(808, 368);
            rtbResponse.Font = new Font("Consolas", 9f);
            rtbResponse.ReadOnly = true;
            rtbResponse.BackColor = Color.FromArgb(18, 18, 18);
            rtbResponse.ForeColor = Color.FromArgb(180, 255, 180);
            rtbResponse.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbResponse.BorderStyle = BorderStyle.None;
            rtbResponse.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            btnClear.Text = "Clear";
            btnClear.Location = new Point(824, 22);
            btnClear.Size = new Size(70, 25);
            btnClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClear.Click += BtnClear_Click;

            grpResponse.Controls.AddRange([rtbResponse, btnClear]);

            // ── Form ──────────────────────────────────────────────────────
            this.Text = "eGK Smart Card Tool";
            this.ClientSize = new Size(920, 658);
            this.MinimumSize = new Size(700, 600);
            this.Controls.AddRange([grpReader, grpCommand, grpServer, grpResponse]);
        }

        #region Fields
        private GroupBox grpReader = null!;
        private Label lblReader = null!;
        private ComboBox cmbReaders = null!;
        private Button btnRefresh = null!;
        private Button btnConnect = null!;
        private Button btnDisconnect = null!;
        private GroupBox grpStatus = null!;
        private Label lblStatusCaption = null!;
        private Label lblStatusValue = null!;
        private Label lblAtrCaption = null!;
        private Label lblAtrValue = null!;
        private Label lblProtocolCaption = null!;
        private Label lblProtocolValue = null!;
        private GroupBox grpCommand = null!;
        private Label lblApduCaption = null!;
        private TextBox txtApdu = null!;
        private Button btnSend = null!;
        private Button btnSelectMF = null!;
        private Button btnGetChallenge = null!;
        private Button btnSelectEfGdo = null!;
        private Button btnReadBinary = null!;
        private Button btnGetResponse = null!;
        private GroupBox grpServer = null!;
        private Button btnStartServer = null!;
        private Button btnStopServer = null!;
        private Label lblServerStatus = null!;
        private GroupBox grpResponse = null!;
        private RichTextBox rtbResponse = null!;
        private Button btnClear = null!;
        #endregion
    }
}
