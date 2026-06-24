using Sudoku.Client.UI;

namespace Sudoku.Client.Forms;

partial class LeaderboardForm
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
    lblSummary = new Label();
    lblDifficulty = new Label();
    cboDifficulty = new ComboBox();
    dgvLeaderboard = new DataGridView();
    btnRefresh = new Button();
    btnClose = new Button();
    SuspendLayout();

    pnlHeader.BackColor = UiTheme.Header;
    pnlHeader.Dock = DockStyle.Top;
    pnlHeader.Height = 72;
    pnlHeader.Controls.Add(lblSummary);
    pnlHeader.Controls.Add(lblTitle);

    lblTitle.AutoSize = true;
    lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
    lblTitle.ForeColor = Color.White;
    lblTitle.Location = new Point(20, 14);
    lblTitle.Text = "Bảng xếp hạng";

    lblSummary.AutoSize = true;
    lblSummary.Font = new Font("Segoe UI", 9F);
    lblSummary.ForeColor = Color.FromArgb(191, 219, 254);
    lblSummary.Location = new Point(22, 44);
    lblSummary.Text = "Chọn chế độ để xem";

    lblDifficulty.AutoSize = true;
    lblDifficulty.Font = UiTheme.LabelFont;
    lblDifficulty.ForeColor = UiTheme.TextMuted;
    lblDifficulty.Location = new Point(20, 88);
    lblDifficulty.Text = "Chế độ";

    cboDifficulty.DropDownStyle = ComboBoxStyle.DropDownList;
    cboDifficulty.Items.AddRange(new object[] { "Dễ — 30 ô trống", "Trung bình — 40 ô trống", "Khó — 50 ô trống" });
    cboDifficulty.Location = new Point(20, 112);
    cboDifficulty.Size = new Size(300, 34);
    UiTheme.StyleComboBox(cboDifficulty);

    dgvLeaderboard.AllowUserToAddRows = false;
    dgvLeaderboard.AllowUserToDeleteRows = false;
    dgvLeaderboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    dgvLeaderboard.BackgroundColor = UiTheme.Card;
    dgvLeaderboard.BorderStyle = BorderStyle.None;
    dgvLeaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    dgvLeaderboard.Location = new Point(20, 158);
    dgvLeaderboard.MultiSelect = false;
    dgvLeaderboard.ReadOnly = true;
    dgvLeaderboard.RowHeadersVisible = false;
    dgvLeaderboard.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgvLeaderboard.Size = new Size(760, 320);
    dgvLeaderboard.Columns.AddRange(
      new DataGridViewTextBoxColumn { HeaderText = "Hạng", FillWeight = 50 },
      new DataGridViewTextBoxColumn { HeaderText = "Người chơi", FillWeight = 140 },
      new DataGridViewTextBoxColumn { HeaderText = "Tổng trận", FillWeight = 80 },
      new DataGridViewTextBoxColumn { HeaderText = "Thắng", FillWeight = 70 },
      new DataGridViewTextBoxColumn { HeaderText = "Tỉ lệ thắng", FillWeight = 90 });

    btnRefresh.Location = new Point(540, 494);
    btnRefresh.Size = new Size(120, 40);
    btnRefresh.Text = "Làm mới";
    btnRefresh.Click += btnRefresh_Click;
    UiTheme.StylePrimaryButton(btnRefresh);

    btnClose.Location = new Point(660, 494);
    btnClose.Size = new Size(120, 40);
    btnClose.Text = "Đóng";
    btnClose.Click += (_, _) => Close();
    UiTheme.StyleSecondaryButton(btnClose);

    AutoScaleDimensions = new SizeF(96F, 96F);
    AutoScaleMode = AutoScaleMode.Dpi;
    ClientSize = new Size(800, 550);
    Controls.Add(btnClose);
    Controls.Add(btnRefresh);
    Controls.Add(dgvLeaderboard);
    Controls.Add(cboDifficulty);
    Controls.Add(lblDifficulty);
    Controls.Add(pnlHeader);
    FormBorderStyle = FormBorderStyle.FixedDialog;
    MaximizeBox = false;
    MinimizeBox = false;
    StartPosition = FormStartPosition.CenterParent;
    Text = "Bảng xếp hạng";
    ResumeLayout(false);
    PerformLayout();
  }

  private Panel pnlHeader;
  private Label lblTitle;
  private Label lblSummary;
  private Label lblDifficulty;
  private ComboBox cboDifficulty;
  private DataGridView dgvLeaderboard;
  private Button btnRefresh;
  private Button btnClose;
}
