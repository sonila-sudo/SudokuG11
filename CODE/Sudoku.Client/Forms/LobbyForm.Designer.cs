using Sudoku.Client.UI;

namespace Sudoku.Client.Forms;

partial class LobbyForm
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
    lblWelcome = new Label();
    lblHeaderSub = new Label();
    pnlStats = new Panel();
    pnlWins = new Panel();
    lblWinsTitle = new Label();
    lblWins = new Label();
    pnlLosses = new Panel();
    lblLossesTitle = new Label();
    lblLosses = new Label();
    pnlTotal = new Panel();
    lblTotalTitle = new Label();
    lblTotal = new Label();
    pnlMain = new Panel();
    lblDifficulty = new Label();
    cboDifficulty = new ComboBox();
    btnFindMatch = new Button();
    lblRoom = new Label();
    txtRoomCode = new TextBox();
    btnCreateRoom = new Button();
    btnJoinRoom = new Button();
    pnlStatusBar = new Panel();
    pnlStatusDot = new Panel();
    lblStatus = new Label();
    SuspendLayout();

    pnlHeader.BackColor = UiTheme.Header;
    pnlHeader.Dock = DockStyle.Top;
    pnlHeader.Height = 100;
    pnlHeader.Padding = new Padding(0, 6, 0, 6);
    pnlHeader.Controls.Add(lblHeaderSub);
    pnlHeader.Controls.Add(lblWelcome);

    lblWelcome.AutoSize = true;
    lblWelcome.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
    lblWelcome.ForeColor = Color.White;
    lblWelcome.Location = new Point(24, 16);

    lblHeaderSub.AutoSize = true;
    lblHeaderSub.Font = new Font("Segoe UI", 10F);
    lblHeaderSub.ForeColor = Color.FromArgb(191, 219, 254);
    lblHeaderSub.Location = new Point(26, 54);
    lblHeaderSub.Text = "Chọn chế độ chơi và bắt đầu thách đấu";

    pnlStats.Location = new Point(24, 116);
    pnlStats.Size = new Size(452, 90);

    SetupStatCard(pnlWins, lblWinsTitle, lblWins, "Thắng", new Point(0, 0), UiTheme.Success);
    SetupStatCard(pnlLosses, lblLossesTitle, lblLosses, "Thua", new Point(154, 0), UiTheme.Danger);
    SetupStatCard(pnlTotal, lblTotalTitle, lblTotal, "Tổng trận", new Point(308, 0), UiTheme.Primary);
    pnlStats.Controls.AddRange(new Control[] { pnlWins, pnlLosses, pnlTotal });

    pnlMain.BackColor = UiTheme.Card;
    pnlMain.Location = new Point(24, 220);
    pnlMain.Size = new Size(452, 280);
    pnlMain.Controls.Add(btnJoinRoom);
    pnlMain.Controls.Add(btnCreateRoom);
    pnlMain.Controls.Add(txtRoomCode);
    pnlMain.Controls.Add(lblRoom);
    pnlMain.Controls.Add(btnFindMatch);
    pnlMain.Controls.Add(cboDifficulty);
    pnlMain.Controls.Add(lblDifficulty);
    pnlMain.Paint += CardBorder;

    lblDifficulty.AutoSize = true;
    lblDifficulty.Font = UiTheme.LabelFont;
    lblDifficulty.ForeColor = UiTheme.TextMuted;
    lblDifficulty.Location = new Point(20, 20);
    lblDifficulty.Text = "Độ khó";

    cboDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
    cboDifficulty.Items.AddRange(new object[] { "Dễ — 30 ô trống", "Trung bình — 40 ô trống", "Khó — 50 ô trống" });
    cboDifficulty.Location = new Point(20, 44);
    cboDifficulty.Size = new Size(412, 34);
    cboDifficulty.SelectedIndex = 1;
    UiTheme.StyleComboBox(cboDifficulty);

    btnFindMatch.Location = new Point(20, 92);
    btnFindMatch.Size = new Size(412, 50);
    btnFindMatch.Text = "Tìm trận nhanh";
    btnFindMatch.Click += btnFindMatch_Click;
    UiTheme.StylePrimaryButton(btnFindMatch);

    lblRoom.AutoSize = true;
    lblRoom.Font = UiTheme.LabelFont;
    lblRoom.ForeColor = UiTheme.TextMuted;
    lblRoom.Location = new Point(20, 156);
    lblRoom.Text = "Phòng riêng (mã 6 ký tự)";

    txtRoomCode.CharacterCasing = CharacterCasing.Upper;
    txtRoomCode.Font = new Font("Consolas", 14F, FontStyle.Bold);
    txtRoomCode.Location = new Point(20, 180);
    txtRoomCode.MaxLength = 6;
    txtRoomCode.Size = new Size(160, 36);
    txtRoomCode.TextAlign = HorizontalAlignment.Center;
    UiTheme.StyleTextBox(txtRoomCode);

    btnCreateRoom.Location = new Point(192, 176);
    btnCreateRoom.Size = new Size(112, 44);
    btnCreateRoom.Text = "Tạo phòng";
    btnCreateRoom.Click += btnCreateRoom_Click;
    UiTheme.StyleSecondaryButton(btnCreateRoom);

    btnJoinRoom.Location = new Point(316, 176);
    btnJoinRoom.Size = new Size(116, 44);
    btnJoinRoom.Text = "Vào phòng";
    btnJoinRoom.Click += btnJoinRoom_Click;
    UiTheme.StyleSecondaryButton(btnJoinRoom);

    pnlStatusBar.Location = new Point(24, 516);
    pnlStatusBar.Size = new Size(452, 32);
    pnlStatusBar.Controls.Add(lblStatus);
    pnlStatusBar.Controls.Add(pnlStatusDot);

    pnlStatusDot.Location = new Point(0, 10);
    pnlStatusDot.Size = new Size(10, 10);
    pnlStatusDot.BackColor = UiTheme.Primary;

    lblStatus.AutoSize = true;
    lblStatus.Font = UiTheme.BodyFont;
    lblStatus.Location = new Point(18, 8);
    lblStatus.ForeColor = UiTheme.TextMuted;
    lblStatus.Text = "Sẵn sàng.";

    AutoScaleDimensions = new SizeF(96F, 96F);
    AutoScaleMode = AutoScaleMode.Dpi;
    ClientSize = new Size(500, 560);
    Controls.Add(pnlStatusBar);
    Controls.Add(pnlMain);
    Controls.Add(pnlStats);
    Controls.Add(pnlHeader);
    Font = new Font("Segoe UI", 10F);
    FormBorderStyle = FormBorderStyle.FixedSingle;
    MaximizeBox = false;
    MinimumSize = new Size(500, 560);
    StartPosition = FormStartPosition.CenterScreen;
    Text = "Sudoku Multiplayer — Sảnh chờ";
    FormClosed += LobbyForm_FormClosed;
    ResumeLayout(false);
    PerformLayout();
  }

  private static void CardBorder(object sender, PaintEventArgs e)
  {
    if (sender is not Panel panel)
      return;
    using var pen = new Pen(UiTheme.Border);
    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
  }

  private static void SetupStatCard(Panel panel, Label title, Label value, string titleText, Point location, Color accent)
  {
    panel.BackColor = UiTheme.Card;
    panel.Location = location;
    panel.Size = new Size(140, 90);
    panel.Padding = new Padding(4);
    panel.Paint += CardBorder;

    title.AutoSize = true;
    title.Font = UiTheme.LabelFont;
    title.ForeColor = UiTheme.TextMuted;
    title.Location = new Point(14, 14);
    title.Text = titleText;

    value.AutoSize = true;
    value.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
    value.ForeColor = accent;
    value.Location = new Point(12, 44);
    value.Text = "0";

    panel.Controls.Add(value);
    panel.Controls.Add(title);
  }

  private Panel pnlHeader;
  private Label lblWelcome;
  private Label lblHeaderSub;
  private Panel pnlStats;
  private Panel pnlWins;
  private Label lblWinsTitle;
  private Label lblWins;
  private Panel pnlLosses;
  private Label lblLossesTitle;
  private Label lblLosses;
  private Panel pnlTotal;
  private Label lblTotalTitle;
  private Label lblTotal;
  private Panel pnlMain;
  private Label lblDifficulty;
  private ComboBox cboDifficulty;
  private Button btnFindMatch;
  private Label lblRoom;
  private TextBox txtRoomCode;
  private Button btnCreateRoom;
  private Button btnJoinRoom;
  private Panel pnlStatusBar;
  private Panel pnlStatusDot;
  private Label lblStatus;
}
