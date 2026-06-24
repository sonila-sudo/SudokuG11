using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sudoku.Shared.Network;

namespace Sudoku.Server.Services;

public sealed class ClientSession
{
  public Guid SessionId { get; } = Guid.NewGuid();
  public TcpClient Client { get; }
  public NetworkStream Stream { get; }
  public StringBuilder Buffer { get; } = new();
  public int PlayerId { get; set; }
  public string Username { get; set; } = string.Empty;
  public GameRoom? CurrentRoom { get; set; }
  public CancellationTokenSource LinkedCts { get; } = new();

  public ClientSession(TcpClient client)
  {
    Client = client;
    Stream = client.GetStream();
  }

  public Task SendAsync(NetworkMessage message) =>
    MessageFramer.SendAsync(Stream, message, LinkedCts.Token);
}

public sealed class SudokuTcpServer
{
  private readonly DatabaseService _database;
  private readonly ConcurrentDictionary<string, GameRoom> _rooms = new();
  private readonly ConcurrentDictionary<Guid, ClientSession> _sessions = new();
  private readonly ConcurrentDictionary<int, ClientSession> _activePlayersById = new();
  private readonly ConcurrentQueue<ClientSession> _matchmakingQueue = new();
  private readonly TcpListener _listener;
  private readonly int _defaultEmptyCells;
  private CancellationTokenSource? _cts;

  public SudokuTcpServer(IPAddress address, int port, DatabaseService database, int defaultEmptyCells = 40)
  {
    _database = database;
    _defaultEmptyCells = defaultEmptyCells;
    _listener = new TcpListener(address, port);
  }

  public async Task RunAsync(CancellationToken externalCt = default)
  {
    _cts = CancellationTokenSource.CreateLinkedTokenSource(externalCt);
    _listener.Start();
    Console.WriteLine($"Sudoku Server đang chạy tại {_listener.LocalEndpoint}");

    while (!_cts.IsCancellationRequested)
    {
      var client = await _listener.AcceptTcpClientAsync(_cts.Token);
      _ = Task.Run(() => HandleClientAsync(new ClientSession(client)), _cts.Token);
    }
  }

  public void Stop()
  {
    _cts?.Cancel();
    _listener.Stop();
  }

  private async Task HandleClientAsync(ClientSession session)
  {
    _sessions[session.SessionId] = session;
    try
    {
      while (!session.LinkedCts.IsCancellationRequested)
      {
        var message = await MessageFramer.ReadAsync(session.Stream, session.Buffer, session.LinkedCts.Token);
        if (message == null)
          break;
        await ProcessMessageAsync(session, message);
      }
    }
    catch (Exception ex) when (ex is IOException or SocketException or OperationCanceledException)
    {
      // client disconnected
    }
    finally
    {
      await CleanupSessionAsync(session);
    }
  }

  private async Task ProcessMessageAsync(ClientSession session, NetworkMessage message)
  {
    try
    {
      await DispatchMessageAsync(session, message);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Lỗi xử lý {message.Type}: {ex}");
      try
      {
        await session.SendAsync(new NetworkMessage
        {
          Type = MessageType.Error,
          Success = false,
          Message = "Server gặp lỗi khi xử lý yêu cầu."
        });
      }
      catch
      {
        // client disconnected
      }
    }
  }

  private async Task DispatchMessageAsync(ClientSession session, NetworkMessage message)
  {
    switch (message.Type)
    {
      case MessageType.Register:
        await HandleRegisterAsync(session, message);
        break;
      case MessageType.Login:
        await HandleLoginAsync(session, message);
        break;
      case MessageType.CreateRoom:
        await HandleCreateRoomAsync(session, message);
        break;
      case MessageType.JoinRoom:
        await HandleJoinRoomAsync(session, message);
        break;
      case MessageType.FindMatch:
        await HandleFindMatchAsync(session, message);
        break;
      case MessageType.CellUpdate:
        await HandleCellUpdateAsync(session, message);
        break;
      case MessageType.GameComplete:
        await HandleGameCompleteAsync(session, message);
        break;
      case MessageType.Surrender:
        await HandleSurrenderAsync(session, message);
        break;
      case MessageType.TooManyMistakes:
        await HandleTooManyMistakesAsync(session, message);
        break;
      case MessageType.LeaveGame:
        await HandleLeaveGameAsync(session);
        break;
      case MessageType.GetMatchHistory:
        await HandleGetMatchHistoryAsync(session);
        break;
      case MessageType.GetLeaderboard:
        await HandleGetLeaderboardAsync(session, message);
        break;
      default:
        await session.SendAsync(new NetworkMessage
        {
          Type = MessageType.Error,
          Success = false,
          Message = "Lệnh không được hỗ trợ."
        });
        break;
    }
  }

  private async Task HandleRegisterAsync(ClientSession session, NetworkMessage message)
  {
    var ok = _database.Register(message.Username ?? string.Empty, message.Password ?? string.Empty, out var error);
    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.RegisterResponse,
      Success = ok,
      Message = ok ? "Đăng ký thành công." : error
    });
  }

  private async Task HandleLoginAsync(ClientSession session, NetworkMessage message)
  {
    var playerId = _database.Login(message.Username ?? string.Empty, message.Password ?? string.Empty);
    if (!playerId.HasValue)
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.LoginResponse,
        Success = false,
        Message = "Sai tên đăng nhập hoặc mật khẩu."
      });
      return;
    }

    session.PlayerId = playerId.Value;
    session.Username = message.Username!.Trim();

    if (_activePlayersById.TryGetValue(playerId.Value, out var existing) &&
        existing.SessionId != session.SessionId)
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.LoginResponse,
        Success = false,
        Message = "Tài khoản đang online ở phiên khác. Hãy đăng xuất trước khi đăng nhập lại."
      });
      return;
    }

    _activePlayersById[playerId.Value] = session;
    var stats = _database.GetStats(playerId.Value);

    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.LoginResponse,
      Success = true,
      Username = session.Username,
      PlayerId = playerId.Value,
      Wins = stats.Wins,
      Losses = stats.Losses,
      TotalGames = stats.TotalGames,
      Message = "Đăng nhập thành công."
    });
  }

  private async Task HandleCreateRoomAsync(ClientSession session, NetworkMessage message)
  {
    if (session.PlayerId == 0)
    {
      await SendAuthErrorAsync(session, MessageType.CreateRoomResponse);
      return;
    }

    var roomCode = GenerateRoomCode();
    var emptyCells = message.EmptyCells > 0 ? message.EmptyCells : _defaultEmptyCells;
    var room = new GameRoom(roomCode, emptyCells, _database);
    if (!room.TryAddPlayer(session))
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.CreateRoomResponse,
        Success = false,
        Message = "Không thể tạo phòng."
      });
      return;
    }

    session.CurrentRoom = room;
    _rooms[roomCode] = room;

    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.CreateRoomResponse,
      Success = true,
      RoomCode = roomCode,
      Message = "Tạo phòng thành công. Chờ đối thủ tham gia..."
    });
  }

  private async Task HandleJoinRoomAsync(ClientSession session, NetworkMessage message)
  {
    if (session.PlayerId == 0)
    {
      await SendAuthErrorAsync(session, MessageType.JoinRoomResponse);
      return;
    }

    var code = message.RoomCode?.Trim().ToUpperInvariant() ?? string.Empty;
    if (!_rooms.TryGetValue(code, out var room))
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.JoinRoomResponse,
        Success = false,
        Message = "Phòng không tồn tại."
      });
      return;
    }

    if (!room.TryAddPlayer(session))
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.JoinRoomResponse,
        Success = false,
        Message = "Phòng đã đủ người."
      });
      return;
    }

    session.CurrentRoom = room;
    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.JoinRoomResponse,
      Success = true,
      RoomCode = code,
      Message = "Đã tham gia phòng."
    });

    await room.StartIfReadyAsync();
  }

  private async Task HandleFindMatchAsync(ClientSession session, NetworkMessage message)
  {
    if (session.PlayerId == 0)
    {
      await SendAuthErrorAsync(session, MessageType.FindMatchResponse);
      return;
    }

    RemoveFromMatchmaking(session);

    ClientSession? opponent = null;
    var skipped = new List<ClientSession>();
    while (_matchmakingQueue.TryDequeue(out var waiting))
    {
      if (waiting.SessionId == session.SessionId || waiting.PlayerId == session.PlayerId)
      {
        skipped.Add(waiting);
        continue;
      }

      opponent = waiting;
      break;
    }

    foreach (var queued in skipped)
      _matchmakingQueue.Enqueue(queued);

    if (opponent != null)
    {
      var roomCode = GenerateRoomCode();
      var emptyCells = message.EmptyCells > 0 ? message.EmptyCells : _defaultEmptyCells;
      var room = new GameRoom(roomCode, emptyCells, _database);
      room.TryAddPlayer(opponent);
      room.TryAddPlayer(session);
      opponent.CurrentRoom = room;
      session.CurrentRoom = room;
      _rooms[roomCode] = room;

      await opponent.SendAsync(new NetworkMessage
      {
        Type = MessageType.FindMatchResponse,
        Success = true,
        Message = "Đã tìm thấy đối thủ!"
      });
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.FindMatchResponse,
        Success = true,
        Message = "Đã tìm thấy đối thủ!"
      });

      await room.StartIfReadyAsync();
      return;
    }

    _matchmakingQueue.Enqueue(session);
    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.FindMatchResponse,
      Success = true,
      Message = "Đang tìm đối thủ..."
    });
  }

  private async Task HandleCellUpdateAsync(ClientSession session, NetworkMessage message)
  {
    if (session.CurrentRoom == null || session.CurrentRoom.IsFinished)
      return;

    var room = session.CurrentRoom;
    if (!room.IsCorrectMove(message.Row, message.Col, message.Value))
      return;

    var count = room.RecordCorrectCell(session, message.Row, message.Col);
    await room.BroadcastProgressAsync(session, count);
  }

  private async Task HandleGameCompleteAsync(ClientSession session, NetworkMessage message)
  {
    if (session.CurrentRoom == null || message.Puzzle == null)
      return;

    if (!IsSolved(message.Puzzle, session.CurrentRoom.Solution))
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.Error,
        Success = false,
        Message = "Bảng chưa hoàn thành hoặc có ô sai."
      });
      return;
    }

    await session.CurrentRoom.FinishMatchAsync(
      session,
      $"{session.Username} đã hoàn thành Sudoku!",
      message.MyElapsedMs,
      null);
  }

  private static bool IsSolved(int[,] board, int[,] solution)
  {
    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
      if (board[r, c] != solution[r, c])
        return false;
    return true;
  }

  private async Task HandleSurrenderAsync(ClientSession session, NetworkMessage message)
  {
    if (session.CurrentRoom == null || session.CurrentRoom.IsFinished)
      return;

    var opponent = session.CurrentRoom.GetOpponent(session);
    if (opponent == null)
      return;

    await session.CurrentRoom.FinishMatchAsync(
      opponent,
      $"{session.Username} đã đầu hàng.",
      null,
      message.MyElapsedMs > 0 ? message.MyElapsedMs : null);
  }

  private async Task HandleLeaveGameAsync(ClientSession session)
  {
    if (session.CurrentRoom == null)
      return;

    var room = session.CurrentRoom;
    if (room.IsStarted && !room.IsFinished)
    {
      Console.WriteLine($"[Game] {session.Username} rời trận giữa chừng.");
      await room.NotifyOpponentDisconnectedAsync(session);
      _rooms.TryRemove(room.RoomCode, out _);
      return;
    }

    if (!room.IsStarted)
    {
      _rooms.TryRemove(room.RoomCode, out _);
      session.CurrentRoom = null;
    }
  }

  private async Task HandleTooManyMistakesAsync(ClientSession session, NetworkMessage message)
  {
    if (session.CurrentRoom == null || session.CurrentRoom.IsFinished)
      return;

    var opponent = session.CurrentRoom.GetOpponent(session);
    if (opponent == null)
      return;

    Console.WriteLine($"[Game] {session.Username} sai quá 3 lỗi — {opponent.Username} thắng.");

    await session.CurrentRoom.FinishMatchAsync(
      opponent,
      $"{session.Username} đã sai quá 3 lỗi.",
      null,
      message.MyElapsedMs > 0 ? message.MyElapsedMs : null);
  }

  private async Task HandleGetMatchHistoryAsync(ClientSession session)
  {
    if (session.PlayerId == 0)
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.GetMatchHistoryResponse,
        Success = false,
        Message = "Bạn cần đăng nhập trước."
      });
      return;
    }

    var history = _database.GetMatchHistory(session.PlayerId);
    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.GetMatchHistoryResponse,
      Success = true,
      History = history
    });
  }

  private async Task HandleGetLeaderboardAsync(ClientSession session, NetworkMessage message)
  {
    if (session.PlayerId == 0)
    {
      await session.SendAsync(new NetworkMessage
      {
        Type = MessageType.GetLeaderboardResponse,
        Success = false,
        Message = "Bạn cần đăng nhập trước."
      });
      return;
    }

    var emptyCells = message.EmptyCells > 0 ? message.EmptyCells : _defaultEmptyCells;
    var leaderboard = _database.GetLeaderboard(emptyCells);
    await session.SendAsync(new NetworkMessage
    {
      Type = MessageType.GetLeaderboardResponse,
      Success = true,
      EmptyCells = emptyCells,
      Leaderboard = leaderboard
    });
  }

  private async Task CleanupSessionAsync(ClientSession session)
  {
    _sessions.TryRemove(session.SessionId, out _);
    RemoveFromMatchmaking(session);

    if (session.PlayerId != 0 &&
        _activePlayersById.TryGetValue(session.PlayerId, out var active) &&
        active.SessionId == session.SessionId)
    {
      _activePlayersById.TryRemove(session.PlayerId, out _);
    }

    if (session.CurrentRoom != null)
    {
      var room = session.CurrentRoom;
      if (room.IsStarted && !room.IsFinished)
      {
        await room.NotifyOpponentDisconnectedAsync(session);
        _rooms.TryRemove(room.RoomCode, out _);
      }
      else if (!room.IsStarted)
      {
        _rooms.TryRemove(room.RoomCode, out _);
        session.CurrentRoom = null;
      }
    }

    session.LinkedCts.Cancel();
    session.Client.Close();
  }

  private void RemoveFromMatchmaking(ClientSession session)
  {
    var remaining = new List<ClientSession>();
    while (_matchmakingQueue.TryDequeue(out var queued))
    {
      if (queued.SessionId != session.SessionId)
        remaining.Add(queued);
    }

    foreach (var item in remaining)
      _matchmakingQueue.Enqueue(item);
  }

  private static async Task SendAuthErrorAsync(ClientSession session, MessageType responseType) =>
    await session.SendAsync(new NetworkMessage
    {
      Type = responseType,
      Success = false,
      Message = "Bạn cần đăng nhập trước."
    });

  private static string GenerateRoomCode()
  {
    const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    var random = Random.Shared;
    return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
  }
}
