using System.Collections.Concurrent;
using Sudoku.Shared.Network;

namespace Sudoku.Server.Services;

public sealed class GameRoom
{
  private readonly object _lock = new();
  private readonly DatabaseService _database;
  private readonly int _emptyCells;
  private readonly DateTime _startedAt = DateTime.UtcNow;
  private readonly ConcurrentDictionary<Guid, HashSet<(int Row, int Col)>> _correctCellsByPlayer = new();

  public string RoomCode { get; }
  public int[,] Puzzle { get; }
  public int[,] Solution { get; }
  public ClientSession? Host { get; private set; }
  public ClientSession? Guest { get; private set; }
  public bool IsFull => Host != null && Guest != null;
  public bool IsStarted { get; private set; }
  public bool IsFinished { get; private set; }

  public GameRoom(string roomCode, int emptyCells, DatabaseService database)
  {
    RoomCode = roomCode;
    _emptyCells = emptyCells;
    _database = database;

    var logic = new Sudoku.Shared.Sudoku.SudokuLogic();
    logic.GenerateRandomBoard(emptyCells);
    Puzzle = (int[,])logic.Board.Clone();
    Solution = (int[,])logic.SolutionBoard.Clone();
  }

  public bool TryAddPlayer(ClientSession session)
  {
    lock (_lock)
    {
      if (Host == null)
      {
        Host = session;
        return true;
      }

      if (Guest == null &&
          Host.SessionId != session.SessionId &&
          Host.PlayerId != session.PlayerId)
      {
        Guest = session;
        return true;
      }

      return false;
    }
  }

  public ClientSession? GetOpponent(ClientSession session)
  {
    lock (_lock)
    {
      if (Host?.SessionId == session.SessionId)
        return Guest;
      if (Guest?.SessionId == session.SessionId)
        return Host;
      return null;
    }
  }

  public async Task StartIfReadyAsync()
  {
    ClientSession? host;
    ClientSession? guest;
    lock (_lock)
    {
      if (!IsFull || IsStarted)
        return;
      IsStarted = true;
      host = Host!;
      guest = Guest!;
    }

    var startForHost = BuildGameStart(host, guest);
    var startForGuest = BuildGameStart(guest, host);
    await host.SendAsync(startForHost);
    await guest.SendAsync(startForGuest);
  }

  private NetworkMessage BuildGameStart(ClientSession me, ClientSession opponent) =>
    new()
    {
      Type = MessageType.GameStart,
      Success = true,
      PlayerId = me.PlayerId,
      OpponentId = opponent.PlayerId,
      OpponentName = opponent.Username,
      Puzzle = Puzzle,
      Solution = Solution,
      EmptyCells = _emptyCells,
      TotalCells = _emptyCells
    };

  public bool IsCorrectMove(int row, int col, int value) =>
    value > 0 && Puzzle[row, col] == 0 && Solution[row, col] == value;

  public int RecordCorrectCell(ClientSession session, int row, int col)
  {
    var cells = _correctCellsByPlayer.GetOrAdd(session.SessionId, _ => new HashSet<(int, int)>());
    lock (cells)
    {
      cells.Add((row, col));
      return cells.Count;
    }
  }

  public async Task BroadcastProgressAsync(ClientSession sender, int correctCells)
  {
    var opponent = GetOpponent(sender);
    if (opponent == null)
      return;

    await opponent.SendAsync(new NetworkMessage
    {
      Type = MessageType.OpponentProgress,
      CorrectCells = correctCells,
      TotalCells = _emptyCells,
      OpponentName = sender.Username
    });
  }

  public async Task FinishMatchAsync(
    ClientSession winner,
    string reason,
    long? winnerElapsedMs = null,
    long? loserElapsedMs = null)
  {
    ClientSession? host;
    ClientSession? guest;
    lock (_lock)
    {
      if (IsFinished)
        return;
      IsFinished = true;
      host = Host;
      guest = Guest;
    }

    if (host == null || guest == null)
      return;

    var loser = GetOpponent(winner);
    if (loser == null)
      return;

    var durationMs = (long)(DateTime.UtcNow - _startedAt).TotalMilliseconds;

    long? hostDuration = null;
    long? guestDuration = null;
    if (host.SessionId == winner.SessionId)
    {
      hostDuration = winnerElapsedMs ?? durationMs;
      guestDuration = loserElapsedMs;
    }
    else
    {
      guestDuration = winnerElapsedMs ?? durationMs;
      hostDuration = loserElapsedMs;
    }

    try
    {
      _database.SaveMatch(
        host.PlayerId,
        guest.PlayerId,
        winner.PlayerId,
        durationMs,
        _emptyCells,
        hostDuration,
        guestDuration);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Lỗi lưu trận: {ex.Message}");
    }

    try
    {
      await SendGameOverAsync(winner, winner, loser, reason);
      await SendGameOverAsync(loser, winner, loser, reason);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Lỗi gửi GameOver: {ex.Message}");
    }

    host.CurrentRoom = null;
    guest.CurrentRoom = null;
  }

  private async Task SendGameOverAsync(
    ClientSession recipient,
    ClientSession winner,
    ClientSession loser,
    string reason)
  {
    var stats = _database.GetStats(recipient.PlayerId);
    var won = recipient.SessionId == winner.SessionId;

    await recipient.SendAsync(new NetworkMessage
    {
      Type = MessageType.GameOver,
      Success = true,
      WinnerName = winner.Username,
      LoserName = loser.Username,
      OpponentName = won ? loser.Username : winner.Username,
      Message = reason,
      Wins = stats.Wins,
      Losses = stats.Losses,
      TotalGames = stats.TotalGames
    });
  }

  public async Task NotifyOpponentDisconnectedAsync(ClientSession disconnected)
  {
    ClientSession? opponent;
    lock (_lock)
    {
      if (IsFinished)
        return;
      IsFinished = true;
      opponent = GetOpponent(disconnected);
    }

    if (opponent == null)
      return;

    await opponent.SendAsync(new NetworkMessage
    {
      Type = MessageType.OpponentDisconnected,
      Message = $"{disconnected.Username} đã rời trận đấu."
    });

    disconnected.CurrentRoom = null;
    opponent.CurrentRoom = null;
  }
}
