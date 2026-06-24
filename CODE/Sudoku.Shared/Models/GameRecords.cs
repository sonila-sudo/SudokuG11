namespace Sudoku.Shared.Models;

public class MatchHistoryEntry
{
  public string OpponentName { get; set; } = string.Empty;
  public string Difficulty { get; set; } = string.Empty;
  public string Result { get; set; } = string.Empty;
  public long DurationMs { get; set; }
  public string FinishedAt { get; set; } = string.Empty;
}

public class LeaderboardEntry
{
  public int Rank { get; set; }
  public string Username { get; set; } = string.Empty;
  public int TotalGames { get; set; }
  public int Wins { get; set; }
  public double WinRate { get; set; }
}

public static class GameDifficulty
{
  public static readonly int[] AllModes = [30, 40, 50];

  public static string LabelFromEmptyCells(int emptyCells) => emptyCells switch
  {
    30 => "Dễ",
    40 => "Trung bình",
    50 => "Khó",
    _ => $"{emptyCells} ô trống"
  };

  public static int EmptyCellsFromIndex(int index) => index switch
  {
    0 => 30,
    1 => 40,
    2 => 50,
    _ => 40
  };
}
