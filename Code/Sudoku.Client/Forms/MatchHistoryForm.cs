using Sudoku.Client.UI;
using Sudoku.Shared.Models;

namespace Sudoku.Client.Forms;

public partial class MatchHistoryForm : Form
{
  public MatchHistoryForm(IReadOnlyList<MatchHistoryEntry> history)
  {
    InitializeComponent();
    UiTheme.ApplyFormStyle(this);
    LoadHistory(history);
  }

  private void LoadHistory(IReadOnlyList<MatchHistoryEntry> history)
  {
    dgvHistory.Rows.Clear();
    foreach (var entry in history)
    {
      dgvHistory.Rows.Add(
        entry.Difficulty,
        entry.OpponentName,
        entry.Result,
        FormatDuration(entry.DurationMs),
        FormatFinishedAt(entry.FinishedAt));
    }

    lblSummary.Text = history.Count == 0
      ? "Chưa có trận đấu nào."
      : $"Hiển thị {history.Count} trận gần nhất.";
  }

  private static string FormatDuration(long ms)
  {
    if (ms <= 0)
      return "—";

    var time = TimeSpan.FromMilliseconds(ms);
    return $"{(int)time.TotalMinutes:00}:{time.Seconds:00}";
  }

  private static string FormatFinishedAt(string iso)
  {
    if (DateTime.TryParse(iso, null, System.Globalization.DateTimeStyles.RoundtripKind, out var dt))
      return dt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    return iso;
  }
}
