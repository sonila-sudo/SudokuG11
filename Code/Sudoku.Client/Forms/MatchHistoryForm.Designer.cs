using Sudoku.Client.UI;

namespace Sudoku.Client.Forms;

partial class MatchHistoryForm
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
    dgvHistory = new DataGridView();
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
    lblTitle.Text = "Lịch sử đấu";

    lblSummary.AutoSize = true;
    lblSummary.Font = new Font("Segoe UI", 9F);
    lblSummary.ForeColor = Color.FromArgb(191, 219, 254);
    lblSummary.Location = new Point(22, 44);
    lblSummary.Text = "Đang tải...";

    dgvHistory.AllowUserToAddRows = false;
    dgvHistory.AllowUserToDeleteRows = false;
    dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
    dgvHistory.BackgroundColor = UiTheme.Card;
    dgvHistory.BorderStyle = BorderStyle.None;
    dgvHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    dgvHistory.Location = new Point(20, 88);
    dgvHistory.MultiSelect = false;
    dgvHistory.ReadOnly = true;
    dgvHistory.RowHeadersVisible = false;
    dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    dgvHistory.Size = new Size(760, 360);
    dgvHistory.Columns.AddRange(
      new DataGridViewTextBoxColumn { HeaderText = "Độ khó", FillWeight = 90 },
      new DataGridViewTextBoxColumn { HeaderText = "Đối thủ", FillWeight = 120 },
      new DataGridViewTextBoxColumn { HeaderText = "Kết quả", FillWeight = 70 },
      new DataGridViewTextBoxColumn { HeaderText = "Thời gian", FillWeight = 80 },
      new DataGridViewTextBoxColumn { HeaderText = "Ngày giờ", FillWeight = 130 });

    btnClose.Location = new Point(660, 462);
    btnClose.Size = new Size(120, 40);
    btnClose.Text = "Đóng";
    UiTheme.StyleSecondaryButton(btnClose);

    AutoScaleDimensions = new SizeF(96F, 96F);
    AutoScaleMode = AutoScaleMode.Dpi;
    ClientSize = new Size(800, 520);
    Controls.Add(btnClose);
    Controls.Add(dgvHistory);
    Controls.Add(pnlHeader);
    FormBorderStyle = FormBorderStyle.FixedDialog;
    MaximizeBox = false;
    MinimizeBox = false;
    StartPosition = FormStartPosition.CenterParent;
    Text = "Lịch sử đấu";
    ResumeLayout(false);
    PerformLayout();
  }

  private Panel pnlHeader;
  private Label lblTitle;
  private Label lblSummary;
  private DataGridView dgvHistory;
  private Button btnClose;
}
