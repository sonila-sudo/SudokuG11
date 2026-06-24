using Microsoft.Data.Sqlite;
using Sudoku.Shared.Models;

namespace Sudoku.Server.Services;

public sealed class DatabaseService : IDisposable
{
  private readonly string _connectionString;

  public DatabaseService(string dbPath)
  {
    _connectionString = $"Data Source={dbPath}";
    Initialize();
  }

  private void Initialize()
  {
    using var connection = Open();
    using var command = connection.CreateCommand();
    command.CommandText =
      """
      CREATE TABLE IF NOT EXISTS Players (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Username TEXT NOT NULL UNIQUE,
        PasswordHash TEXT NOT NULL,
        Wins INTEGER NOT NULL DEFAULT 0,
        Losses INTEGER NOT NULL DEFAULT 0,
        TotalGames INTEGER NOT NULL DEFAULT 0,
        CreatedAt TEXT NOT NULL
      );

      CREATE TABLE IF NOT EXISTS Matches (
        Id INTEGER PRIMARY KEY AUTOINCREMENT,
        Player1Id INTEGER NOT NULL,
        Player2Id INTEGER NOT NULL,
        WinnerId INTEGER,
        DurationMs INTEGER NOT NULL,
        EmptyCells INTEGER NOT NULL,
        FinishedAt TEXT NOT NULL,
        FOREIGN KEY(Player1Id) REFERENCES Players(Id),
        FOREIGN KEY(Player2Id) REFERENCES Players(Id),
        FOREIGN KEY(WinnerId) REFERENCES Players(Id)
      );
      """;
    command.ExecuteNonQuery();
    EnsureMatchDurationColumns(connection);
  }

  private static void EnsureMatchDurationColumns(SqliteConnection connection)
  {
    var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    using (var info = connection.CreateCommand())
    {
      info.CommandText = "PRAGMA table_info(Matches)";
      using var reader = info.ExecuteReader();
      while (reader.Read())
        columns.Add(reader.GetString(1));
    }

    if (!columns.Contains("Player1DurationMs"))
    {
      using var alter = connection.CreateCommand();
      alter.CommandText = "ALTER TABLE Matches ADD COLUMN Player1DurationMs INTEGER";
      alter.ExecuteNonQuery();
    }

    if (!columns.Contains("Player2DurationMs"))
    {
      using var alter = connection.CreateCommand();
      alter.CommandText = "ALTER TABLE Matches ADD COLUMN Player2DurationMs INTEGER";
      alter.ExecuteNonQuery();
    }
  }

  public bool Register(string username, string password, out string error)
  {
    error = string.Empty;
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
      error = "Tên đăng nhập và mật khẩu không được để trống.";
      return false;
    }

    try
    {
      using var connection = Open();
      using var command = connection.CreateCommand();
      command.CommandText =
        "INSERT INTO Players (Username, PasswordHash, CreatedAt) VALUES ($u, $p, $c)";
      command.Parameters.AddWithValue("$u", username.Trim());
      command.Parameters.AddWithValue("$p", HashPassword(password));
      command.Parameters.AddWithValue("$c", DateTime.UtcNow.ToString("O"));
      command.ExecuteNonQuery();
      return true;
    }
    catch (SqliteException ex) when (ex.SqliteErrorCode == 19)
    {
      error = "Tên đăng nhập đã tồn tại.";
      return false;
    }
  }

  public int? Login(string username, string password)
  {
    using var connection = Open();
    using var command = connection.CreateCommand();
    command.CommandText = "SELECT Id, PasswordHash FROM Players WHERE Username = $u";
    command.Parameters.AddWithValue("$u", username.Trim());

    using var reader = command.ExecuteReader();
    if (!reader.Read())
      return null;

    var id = reader.GetInt32(0);
    var hash = reader.GetString(1);
    return hash == HashPassword(password) ? id : null;
  }

  public (int Wins, int Losses, int TotalGames) GetStats(int playerId)
  {
    using var connection = Open();
    using var command = connection.CreateCommand();
    command.CommandText = "SELECT Wins, Losses, TotalGames FROM Players WHERE Id = $id";
    command.Parameters.AddWithValue("$id", playerId);

    using var reader = command.ExecuteReader();
    if (!reader.Read())
      return (0, 0, 0);

    return (reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2));
  }

  public string? GetUsername(int playerId)
  {
    using var connection = Open();
    using var command = connection.CreateCommand();
    command.CommandText = "SELECT Username FROM Players WHERE Id = $id";
    command.Parameters.AddWithValue("$id", playerId);
    return command.ExecuteScalar() as string;
  }

  public void SaveMatch(
    int player1Id,
    int player2Id,
    int? winnerId,
    long durationMs,
    int emptyCells,
    long? player1DurationMs,
    long? player2DurationMs)
  {
    using var connection = Open();
    using var transaction = connection.BeginTransaction();

    using (var match = connection.CreateCommand())
    {
      match.Transaction = transaction;
      match.CommandText =
        """
        INSERT INTO Matches (Player1Id, Player2Id, WinnerId, DurationMs, EmptyCells, FinishedAt, Player1DurationMs, Player2DurationMs)
        VALUES ($p1, $p2, $w, $d, $e, $f, $d1, $d2)
        """;
      match.Parameters.AddWithValue("$p1", player1Id);
      match.Parameters.AddWithValue("$p2", player2Id);
      match.Parameters.AddWithValue("$w", winnerId.HasValue ? winnerId.Value : DBNull.Value);
      match.Parameters.AddWithValue("$d", durationMs);
      match.Parameters.AddWithValue("$e", emptyCells);
      match.Parameters.AddWithValue("$f", DateTime.UtcNow.ToString("O"));
      match.Parameters.AddWithValue("$d1", player1DurationMs.HasValue ? player1DurationMs.Value : DBNull.Value);
      match.Parameters.AddWithValue("$d2", player2DurationMs.HasValue ? player2DurationMs.Value : DBNull.Value);
      match.ExecuteNonQuery();
    }

    UpdatePlayerStats(connection, transaction, player1Id, winnerId == player1Id);
    UpdatePlayerStats(connection, transaction, player2Id, winnerId == player2Id);
    transaction.Commit();
  }

  public List<MatchHistoryEntry> GetMatchHistory(int playerId, int limit = 50)
  {
    using var connection = Open();
    using var command = connection.CreateCommand();
    command.CommandText =
      """
      SELECT
        CASE WHEN m.Player1Id = $id THEN p2.Username ELSE p1.Username END AS OpponentName,
        m.EmptyCells,
        m.WinnerId,
        m.Player1Id,
        m.Player1DurationMs,
        m.Player2DurationMs,
        m.FinishedAt
      FROM Matches m
      JOIN Players p1 ON m.Player1Id = p1.Id
      JOIN Players p2 ON m.Player2Id = p2.Id
      WHERE m.Player1Id = $id OR m.Player2Id = $id
      ORDER BY m.FinishedAt DESC
      LIMIT $limit
      """;
    command.Parameters.AddWithValue("$id", playerId);
    command.Parameters.AddWithValue("$limit", limit);

    var history = new List<MatchHistoryEntry>();
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
      var isPlayer1 = reader.GetInt32(3) == playerId;
      var winnerId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
      var myDuration = isPlayer1
        ? (reader.IsDBNull(4) ? 0L : reader.GetInt64(4))
        : (reader.IsDBNull(5) ? 0L : reader.GetInt64(5));

      history.Add(new MatchHistoryEntry
      {
        OpponentName = reader.GetString(0),
        Difficulty = GameDifficulty.LabelFromEmptyCells(reader.GetInt32(1)),
        Result = winnerId == playerId ? "Thắng" : "Thua",
        DurationMs = myDuration,
        FinishedAt = reader.GetString(6)
      });
    }

    return history;
  }

  public List<LeaderboardEntry> GetLeaderboard(int emptyCells, int limit = 100)
  {
    using var connection = Open();
    using var command = connection.CreateCommand();
    command.CommandText =
      """
      SELECT
        p.Username,
        COUNT(*) AS TotalGames,
        SUM(CASE WHEN m.WinnerId = p.Id THEN 1 ELSE 0 END) AS Wins
      FROM Players p
      JOIN Matches m ON m.Player1Id = p.Id OR m.Player2Id = p.Id
      WHERE m.EmptyCells = $empty
      GROUP BY p.Id
      HAVING TotalGames > 0
      ORDER BY Wins DESC, (CAST(Wins AS REAL) / TotalGames) DESC, TotalGames DESC
      LIMIT $limit
      """;
    command.Parameters.AddWithValue("$empty", emptyCells);
    command.Parameters.AddWithValue("$limit", limit);

    var entries = new List<LeaderboardEntry>();
    using var reader = command.ExecuteReader();
    var rank = 1;
    while (reader.Read())
    {
      var totalGames = reader.GetInt32(1);
      var wins = reader.GetInt32(2);
      entries.Add(new LeaderboardEntry
      {
        Rank = rank++,
        Username = reader.GetString(0),
        TotalGames = totalGames,
        Wins = wins,
        WinRate = totalGames > 0 ? Math.Round(wins * 100.0 / totalGames, 1) : 0
      });
    }

    return entries;
  }

  private static void UpdatePlayerStats(SqliteConnection connection, SqliteTransaction transaction, int playerId, bool won)
  {
    using var command = connection.CreateCommand();
    command.Transaction = transaction;
    command.CommandText =
      """
      UPDATE Players
      SET TotalGames = TotalGames + 1,
          Wins = Wins + $w,
          Losses = Losses + $l
      WHERE Id = $id
      """;
    command.Parameters.AddWithValue("$w", won ? 1 : 0);
    command.Parameters.AddWithValue("$l", won ? 0 : 1);
    command.Parameters.AddWithValue("$id", playerId);
    command.ExecuteNonQuery();
  }

  private SqliteConnection Open()
  {
    var connection = new SqliteConnection(_connectionString);
    connection.Open();
    return connection;
  }

  private static string HashPassword(string password) =>
    Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(
      System.Text.Encoding.UTF8.GetBytes(password)));

  public void Dispose() { }
}
