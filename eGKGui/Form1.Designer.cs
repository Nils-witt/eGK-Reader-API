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

            btnConnect.Text = "Connect";
            btnConnect.Location = new Point(687, 21);
            btnConnect.Size = new Size(80, 25);
            btnConnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConnect.Click += BtnConnect_Click;

            btnDisconnect.Text = "Disconnect";
            btnDisconnect.Location = new Point(775, 21);
            btnDisconnect.Size = new Size(88, 25);
            btnDisconnect.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDisconnect.Enabled = false;
            btnDisconnect.Click += BtnDisconnect_Click;

            grpReader.Controls.AddRange([lblReader, cmbReaders, btnRefresh, btnConnect, btnDisconnect]);

            // ── GroupBox: Card Status ─────────────────────────────────────
            grpStatus.Text = "Card Status";
            grpStatus.Location = new Point(8, 74);
            grpStatus.Size = new Size(904, 72);
            grpStatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblStatusCaption.Text = "Status:";
            lblStatusCaption.Location = new Point(8, 24);
            lblStatusCaption.Size = new Size(50, 23);
            lblStatusCaption.TextAlign = ContentAlignment.MiddleLeft;

            lblStatusValue.Text = "Not connected";
            lblStatusValue.Location = new Point(62, 24);
            lblStatusValue.Size = new Size(180, 23);
            lblStatusValue.ForeColor = Color.Gray;

            lblAtrCaption.Text = "ATR:";
            lblAtrCaption.Location = new Point(250, 24);
            lblAtrCaption.Size = new Size(32, 23);
            lblAtrCaption.TextAlign = ContentAlignment.MiddleLeft;

            lblAtrValue.Text = "—";
            lblAtrValue.Location = new Point(286, 24);
            lblAtrValue.Size = new Size(608, 23);
            lblAtrValue.Font = new Font("Consolas", 9f);
            lblAtrValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            lblProtocolCaption.Text = "Protocol:";
            lblProtocolCaption.Location = new Point(8, 48);
            lblProtocolCaption.Size = new Size(60, 23);
            lblProtocolCaption.TextAlign = ContentAlignment.MiddleLeft;

            lblProtocolValue.Text = "—";
            lblProtocolValue.Location = new Point(72, 48);
            lblProtocolValue.Size = new Size(120, 23);

            grpStatus.Controls.AddRange([lblStatusCaption, lblStatusValue, lblAtrCaption, lblAtrValue, lblProtocolCaption, lblProtocolValue]);

            // ── GroupBox: APDU Command ────────────────────────────────────
            grpCommand.Text = "APDU Command";
            grpCommand.Location = new Point(8, 154);
            grpCommand.Size = new Size(904, 88);
            grpCommand.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            btnSelectEfGdo.Text = "eGK";
            btnSelectEfGdo.Location = new Point(8, 54);
            btnSelectEfGdo.Size = new Size(300, 25);
            btnSelectEfGdo.Click += BtnReadeGK;


            grpCommand.Controls.AddRange([lblApduCaption,btnSelectEfGdo]);

            // ── GroupBox: Response Log ────────────────────────────────────
            grpResponse.Text = "Response Log";
            grpResponse.Location = new Point(8, 250);
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
            this.Controls.AddRange([grpReader, grpStatus, grpCommand, grpResponse]);
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
        private GroupBox grpResponse = null!;
        private RichTextBox rtbResponse = null!;
        private Button btnClear = null!;
        #endregion
    }
}
