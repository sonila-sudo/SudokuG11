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

  public async Task FinishMatchAsync(ClientSession winner, string reason)
  {
    lock (_lock)
    {
      if (IsFinished)
        return;
      IsFinished = true;
    }

    ClientSession? host;
    ClientSession? guest;
    lock (_lock)
    {
      host = Host;
      guest = Guest;
    }

    if (host == null || guest == null)
      return;

    var loser = GetOpponent(winner);
    var durationMs = (long)(DateTime.UtcNow - _startedAt).TotalMilliseconds;
    _database.SaveMatch(host.PlayerId, guest.PlayerId, winner.PlayerId, durationMs, _emptyCells);

    var winnerStats = _database.GetStats(winner.PlayerId);
    var gameOver = new NetworkMessage
    {
      Type = MessageType.GameOver,
      Success = true,
      WinnerName = winner.Username,
      Message = reason,
      Wins = winnerStats.Wins,
      Losses = winnerStats.Losses,
      TotalGames = winnerStats.TotalGames
    };

    await winner.SendAsync(gameOver);

    if (loser != null)
    {
      var loserStats = _database.GetStats(loser.PlayerId);
      await loser.SendAsync(new NetworkMessage
      {
        Type = MessageType.GameOver,
        Success = true,
        WinnerName = winner.Username,
        Message = reason,
        Wins = loserStats.Wins,
        Losses = loserStats.Losses,
        TotalGames = loserStats.TotalGames
      });
    }
  }

  public async Task NotifyOpponentDisconnectedAsync(ClientSession disconnected)
  {
    var opponent = GetOpponent(disconnected);
    if (opponent == null)
      return;

    await opponent.SendAsync(new NetworkMessage
    {
      Type = MessageType.OpponentDisconnected,
      Message = $"{disconnected.Username} đã ngắt kết nối."
    });
  }
}
