using Sudoku.Client.UI;

namespace Sudoku.Client.Forms;

partial class GameForm
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
    pnlBoard = new Panel();
    pnlSidebar = new Panel();
    lblMatchTitle = new Label();
    pnlTimer = new Panel();
    lblTimerCaption = new Label();
    lblMyTime = new Label();
    pnlProgress = new Panel();
    lblProgressCaption = new Label();
    lblMyProgress = new Label();
    progressMy = new ProgressBar();
    lblOpponentProgress = new Label();
    progressOpponent = new ProgressBar();
    pnlOpponent = new Panel();
    lblOpponentCaption = new Label();
    lblOpponent = new Label();
    lblGameStatus = new Label();
    btnSurrender = new Button();
    SuspendLayout();

    UiTheme.ApplyFormStyle(this);

    pnlBoard.Location = new Point(20, 20);
    pnlBoard.Size = new Size(590, 590);

    pnlSidebar.BackColor = UiTheme.Card;
    pnlSidebar.Location = new Point(634, 20);
    pnlSidebar.Size = new Size(280, 590);
    pnlSidebar.Controls.Add(btnSurrender);
    pnlSidebar.Controls.Add(lblGameStatus);
    pnlSidebar.Controls.Add(pnlOpponent);
    pnlSidebar.Controls.Add(pnlProgress);
    pnlSidebar.Controls.Add(pnlTimer);
    pnlSidebar.Controls.Add(lblMatchTitle);

    lblMatchTitle.AutoSize = true;
    lblMatchTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
    lblMatchTitle.ForeColor = UiTheme.Text;
    lblMatchTitle.Location = new Point(20, 20);
    lblMatchTitle.Text = "Trận đấu";

    pnlTimer.BackColor = Color.FromArgb(239, 246, 255);
    pnlTimer.Location = new Point(20, 60);
    pnlTimer.Size = new Size(240, 96);
    pnlTimer.Controls.Add(lblMyTime);
    pnlTimer.Controls.Add(lblTimerCaption);

    lblTimerCaption.AutoSize = true;
    lblTimerCaption.Font = UiTheme.LabelFont;
    lblTimerCaption.ForeColor = UiTheme.TextMuted;
    lblTimerCaption.Location = new Point(14, 12);
    lblTimerCaption.Text = "Thời gian của bạn";

    lblMyTime.AutoSize = true;
    lblMyTime.Font = new Font("Consolas", 28F, FontStyle.Bold);
    lblMyTime.ForeColor = UiTheme.Primary;
    lblMyTime.Location = new Point(12, 38);
    lblMyTime.Text = "00:00";

    pnlProgress.BackColor = Color.FromArgb(240, 253, 244);
    pnlProgress.Location = new Point(20, 172);
    pnlProgress.Size = new Size(240, 130);
    pnlProgress.Controls.Add(progressOpponent);
    pnlProgress.Controls.Add(lblOpponentProgress);
    pnlProgress.Controls.Add(progressMy);
    pnlProgress.Controls.Add(lblMyProgress);
    pnlProgress.Controls.Add(lblProgressCaption);

    lblProgressCaption.AutoSize = true;
    lblProgressCaption.Font = UiTheme.LabelFont;
    lblProgressCaption.ForeColor = UiTheme.TextMuted;
    lblProgressCaption.Location = new Point(14, 10);
    lblProgressCaption.Text = "Tiến độ giải";

    lblMyProgress.AutoSize = true;
    lblMyProgress.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
    lblMyProgress.ForeColor = UiTheme.Primary;
    lblMyProgress.Location = new Point(14, 36);
    lblMyProgress.Text = "Bạn: 0/40 ô";

    progressMy.Location = new Point(14, 58);
    progressMy.Size = new Size(212, 14);
    progressMy.Maximum = 50;
    progressMy.Style = ProgressBarStyle.Continuous;

    lblOpponentProgress.AutoSize = true;
    lblOpponentProgress.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
    lblOpponentProgress.ForeColor = Color.FromArgb(194, 65, 12);
    lblOpponentProgress.Location = new Point(14, 82);
    lblOpponentProgress.Text = "Đối thủ: 0/40 ô";

    progressOpponent.Location = new Point(14, 104);
    progressOpponent.Size = new Size(212, 14);
    progressOpponent.Maximum = 50;
    progressOpponent.ForeColor = Color.FromArgb(249, 115, 22);
    progressOpponent.Style = ProgressBarStyle.Continuous;

    pnlOpponent.BackColor = Color.FromArgb(255, 247, 237);
    pnlOpponent.Location = new Point(20, 318);
    pnlOpponent.Size = new Size(240, 72);
    pnlOpponent.Controls.Add(lblOpponent);
    pnlOpponent.Controls.Add(lblOpponentCaption);

    lblOpponentCaption.AutoSize = true;
    lblOpponentCaption.Font = UiTheme.LabelFont;
    lblOpponentCaption.ForeColor = UiTheme.TextMuted;
    lblOpponentCaption.Location = new Point(14, 10);
    lblOpponentCaption.Text = "Đối thủ";

    lblOpponent.AutoSize = true;
    lblOpponent.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
    lblOpponent.ForeColor = Color.FromArgb(194, 65, 12);
    lblOpponent.Location = new Point(14, 34);
    lblOpponent.Text = "—";

    lblGameStatus.AutoSize = true;
    lblGameStatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
    lblGameStatus.ForeColor = UiTheme.Success;
    lblGameStatus.Location = new Point(20, 408);
    lblGameStatus.MaximumSize = new Size(240, 0);
    lblGameStatus.Text = "● Đang chơi...";

    btnSurrender.Location = new Point(20, 520);
    btnSurrender.Size = new Size(240, 48);
    btnSurrender.Text = "Đầu hàng";
    btnSurrender.Click += btnSurrender_Click;
    UiTheme.StyleDangerButton(btnSurrender);

    AutoScaleDimensions = new SizeF(10F, 25F);
    AutoScaleMode = AutoScaleMode.Font;
    ClientSize = new Size(934, 634);
    Controls.Add(pnlSidebar);
    Controls.Add(pnlBoard);
    MinimumSize = new Size(950, 680);
    StartPosition = FormStartPosition.CenterScreen;
    Text = "Sudoku Multiplayer — Trận đấu";
    Load += GameForm_Load;
    FormClosed += GameForm_FormClosed;
    ResumeLayout(false);
    PerformLayout();
  }

  private Panel pnlBoard;
  private Panel pnlSidebar;
  private Label lblMatchTitle;
  private Panel pnlTimer;
  private Label lblTimerCaption;
  private Label lblMyTime;
  private Panel pnlProgress;
  private Label lblProgressCaption;
  private Label lblMyProgress;
  private ProgressBar progressMy;
  private Label lblOpponentProgress;
  private ProgressBar progressOpponent;
  private Panel pnlOpponent;
  private Label lblOpponentCaption;
  private Label lblOpponent;
  private Label lblGameStatus;
  private Button btnSurrender;
}
