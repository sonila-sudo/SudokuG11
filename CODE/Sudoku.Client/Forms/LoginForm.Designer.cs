using Sudoku.Client.UI;

namespace Sudoku.Client.Forms;

partial class LoginForm
{
  private System.ComponentModel.IContainer components = null;

  protected override void Dispose(bool disposing)
  {
    if (disposing && components != null)
      components.Dispose();
    base.Dispose(disposing);
  }

  private void InitializeComponent()
  {
    pnlHeader = new Panel();
    lblTitle = new Label();
    lblSubtitle = new Label();
    pnlCard = new Panel();
    lblUsername = new Label();
    txtUsername = new TextBox();
    lblPassword = new Label();
    txtPassword = new TextBox();
    btnLogin = new Button();
    btnRegister = new Button();
    pnlServer = new Panel();
    lblServerTitle = new Label();
    lblServerHost = new Label();
    txtServerHost = new TextBox();
    lblServerPort = new Label();
    txtServerPort = new TextBox();
    btnRetry = new Button();
    pnlStatus = new Panel();
    pnlStatusIndicator = new Panel();
    lblStatus = new Label();
    SuspendLayout();

    pnlHeader.BackColor = UiTheme.Header;
    pnlHeader.Dock = DockStyle.Top;
    pnlHeader.Height = 120;
    pnlHeader.Padding = new Padding(0, 8, 0, 8);
    pnlHeader.Controls.Add(lblSubtitle);
    pnlHeader.Controls.Add(lblTitle);

    lblTitle.AutoSize = true;
    lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
    lblTitle.ForeColor = Color.White;
    lblTitle.Location = new Point(32, 20);
    lblTitle.Text = "Sudoku Multiplayer";

    lblSubtitle.AutoSize = true;
    lblSubtitle.Font = new Font("Segoe UI", 10F);
    lblSubtitle.ForeColor = Color.FromArgb(191, 219, 254);
    lblSubtitle.Location = new Point(34, 72);
    lblSubtitle.Text = "Thách đấu Sudoku trực tuyến — 2 người chơi";

    pnlCard.BackColor = UiTheme.Card;
    pnlCard.Location = new Point(28, 140);
    pnlCard.Size = new Size(464, 230);
    pnlCard.Controls.Add(btnRegister);
    pnlCard.Controls.Add(btnLogin);
    pnlCard.Controls.Add(txtPassword);
    pnlCard.Controls.Add(lblPassword);
    pnlCard.Controls.Add(txtUsername);
    pnlCard.Controls.Add(lblUsername);

    lblUsername.AutoSize = true;
    lblUsername.Font = UiTheme.LabelFont;
    lblUsername.ForeColor = UiTheme.TextMuted;
    lblUsername.Location = new Point(24, 20);
    lblUsername.Text = "Tên đăng nhập";

    txtUsername.Location = new Point(24, 44);
    txtUsername.Size = new Size(416, 34);
    UiTheme.StyleTextBox(txtUsername);

    lblPassword.AutoSize = true;
    lblPassword.Font = UiTheme.LabelFont;
    lblPassword.ForeColor = UiTheme.TextMuted;
    lblPassword.Location = new Point(24, 90);
    lblPassword.Text = "Mật khẩu";

    txtPassword.Location = new Point(24, 114);
    txtPassword.Size = new Size(416, 34);
    txtPassword.UseSystemPasswordChar = true;
    UiTheme.StyleTextBox(txtPassword);

    btnLogin.Location = new Point(24, 168);
    btnLogin.Size = new Size(200, 46);
    btnLogin.Text = "Đăng nhập";
    btnLogin.Click += btnLogin_Click;
    UiTheme.StylePrimaryButton(btnLogin);

    btnRegister.Location = new Point(240, 168);
    btnRegister.Size = new Size(200, 46);
    btnRegister.Text = "Đăng ký";
    btnRegister.Click += btnRegister_Click;
    UiTheme.StyleSecondaryButton(btnRegister);

    pnlServer.BackColor = Color.FromArgb(248, 250, 252);
    pnlServer.Location = new Point(28, 386);
    pnlServer.Size = new Size(464, 120);
    pnlServer.Controls.Add(btnRetry);
    pnlServer.Controls.Add(txtServerPort);
    pnlServer.Controls.Add(lblServerPort);
    pnlServer.Controls.Add(txtServerHost);
    pnlServer.Controls.Add(lblServerHost);
    pnlServer.Controls.Add(lblServerTitle);

    lblServerTitle.AutoSize = true;
    lblServerTitle.Font = UiTheme.LabelFont;
    lblServerTitle.ForeColor = UiTheme.Text;
    lblServerTitle.Location = new Point(16, 14);
    lblServerTitle.Text = "Cấu hình server";

    lblServerHost.AutoSize = true;
    lblServerHost.Font = UiTheme.LabelFont;
    lblServerHost.ForeColor = UiTheme.TextMuted;
    lblServerHost.Location = new Point(16, 44);
    lblServerHost.Text = "Địa chỉ";

    txtServerHost.Location = new Point(16, 66);
    txtServerHost.Size = new Size(240, 34);
    txtServerHost.Text = "127.0.0.1";
    UiTheme.StyleTextBox(txtServerHost);

    lblServerPort.AutoSize = true;
    lblServerPort.Font = UiTheme.LabelFont;
    lblServerPort.ForeColor = UiTheme.TextMuted;
    lblServerPort.Location = new Point(268, 44);
    lblServerPort.Text = "Cổng";

    txtServerPort.Location = new Point(268, 66);
    txtServerPort.Size = new Size(80, 34);
    txtServerPort.Text = "5050";
    UiTheme.StyleTextBox(txtServerPort);

    btnRetry.Location = new Point(360, 62);
    btnRetry.Size = new Size(88, 40);
    btnRetry.Text = "Thử lại";
    btnRetry.Click += btnRetry_Click;
    UiTheme.StyleSecondaryButton(btnRetry);

    pnlStatus.Location = new Point(28, 520);
    pnlStatus.Size = new Size(464, 32);
    pnlStatus.Controls.Add(lblStatus);
    pnlStatus.Controls.Add(pnlStatusIndicator);

    pnlStatusIndicator.Location = new Point(0, 10);
    pnlStatusIndicator.Size = new Size(12, 12);
    pnlStatusIndicator.BackColor = UiTheme.Danger;

    lblStatus.AutoSize = true;
    lblStatus.Location = new Point(20, 8);
    lblStatus.Font = UiTheme.BodyFont;
    lblStatus.ForeColor = UiTheme.TextMuted;
    lblStatus.Text = "Đang khởi tạo...";

    AutoScaleDimensions = new SizeF(96F, 96F);
    AutoScaleMode = AutoScaleMode.Dpi;
    ClientSize = new Size(520, 570);
    Controls.Add(pnlStatus);
    Controls.Add(pnlServer);
    Controls.Add(pnlCard);
    Controls.Add(pnlHeader);
    Font = new Font("Segoe UI", 10F);
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox = false;
    MinimumSize = new Size(520, 570);
    StartPosition = FormStartPosition.CenterScreen;
    Text = "Sudoku Multiplayer — Đăng nhập";
    Load += LoginForm_Load;
    FormClosing += LoginForm_FormClosing;
    ResumeLayout(false);
    PerformLayout();
  }

  private Panel pnlHeader;
  private Label lblTitle;
  private Label lblSubtitle;
  private Panel pnlCard;
  private Label lblUsername;
  private TextBox txtUsername;
  private Label lblPassword;
  private TextBox txtPassword;
  private Button btnLogin;
  private Button btnRegister;
  private Panel pnlServer;
  private Label lblServerTitle;
  private Label lblServerHost;
  private TextBox txtServerHost;
  private Label lblServerPort;
  private TextBox txtServerPort;
  private Button btnRetry;
  private Panel pnlStatus;
  private Panel pnlStatusIndicator;
  private Label lblStatus;
}
