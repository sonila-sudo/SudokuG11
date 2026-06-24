using Sudoku.Client.Network;
using Sudoku.Client.UI;
using Sudoku.Shared.Models;

namespace Sudoku.Client.Forms;

public partial class LeaderboardForm : Form
{
  private readonly GameClient _client;

  public LeaderboardForm(GameClient client)
  {
    _client = client;
    InitializeComponent();
    btnClose.Click += (_, _) => Close();
    UiTheme.ApplyFormStyle(this);
    cboDifficulty.SelectedIndex = 1;
    cboDifficulty.SelectedIndexChanged += async (_, _) => await LoadLeaderboardAsync();
    Shown += async (_, _) => await LoadLeaderboardAsync();
  }

  private int GetDifficulty() => GameDifficulty.EmptyCellsFromIndex(cboDifficulty.SelectedIndex);

  private async Task LoadLeaderboardAsync()
  {
    btnRefresh.Enabled = false;
    lblSummary.Text = "Đang tải bảng xếp hạng...";

    try
    {
      var (success, message, leaderboard) = await _client.FetchLeaderboardAsync(GetDifficulty());
      if (!success)
      {
        MessageBox.Show(message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        lblSummary.Text = "Không tải được bảng xếp hạng.";
        return;
      }

      dgvLeaderboard.Rows.Clear();
      foreach (var entry in leaderboard)
      {
        dgvLeaderboard.Rows.Add(
          entry.Rank,
          entry.Username,
          entry.TotalGames,
          entry.Wins,
          $"{entry.WinRate:0.0}%");
      }

      lblSummary.Text = leaderboard.Count == 0
        ? $"Chưa có dữ liệu cho chế độ {GameDifficulty.LabelFromEmptyCells(GetDifficulty())}."
        : $"Bảng xếp hạng — {GameDifficulty.LabelFromEmptyCells(GetDifficulty())} ({leaderboard.Count} người chơi)";
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
      lblSummary.Text = "Không tải được bảng xếp hạng.";
    }
    finally
    {
      btnRefresh.Enabled = true;
    }
  }

  private async void btnRefresh_Click(object sender, EventArgs e) => await LoadLeaderboardAsync();
}
