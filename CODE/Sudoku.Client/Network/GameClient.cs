using System.Net.Sockets;
using System.Text;
using Sudoku.Shared.Network;

namespace Sudoku.Client.Network;

public sealed class GameClient : IDisposable
{
  private TcpClient? _client;
  private NetworkStream? _stream;
  private readonly StringBuilder _buffer = new();
  private CancellationTokenSource? _readCts;
  private Task? _readTask;

  public string ServerHost { get; set; } = "127.0.0.1";
  public int ServerPort { get; set; } = 5050;
  public bool IsConnected => _client?.Connected == true;
  public string Username { get; private set; } = string.Empty;
  public int PlayerId { get; private set; }
  public int Wins { get; private set; }
  public int Losses { get; private set; }
  public int TotalGames { get; private set; }

  public event Action<NetworkMessage>? MessageReceived;
  public event Action? Disconnected;

  public async Task ConnectAsync()
  {
    Disconnect();

    _client = new TcpClient();
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
    await _client.ConnectAsync(ServerHost, ServerPort, cts.Token);
    _stream = _client.GetStream();
    _readCts = new CancellationTokenSource();
    _readTask = Task.Run(() => ReadLoopAsync(_readCts.Token));
  }

  public void Disconnect()
  {
    _readCts?.Cancel();
    _stream?.Dispose();
    _client?.Close();
    _readCts = null;
    _stream = null;
    _client = null;
    _buffer.Clear();
  }

  public async Task<NetworkMessage> SendAndWaitAsync(NetworkMessage request, MessageType expectedResponse, int timeoutMs = 10000)
  {
    var tcs = new TaskCompletionSource<NetworkMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
    void Handler(NetworkMessage msg)
    {
      if (msg.Type == expectedResponse)
        tcs.TrySetResult(msg);
    }

    MessageReceived += Handler;
    try
    {
      await SendAsync(request);
      var completed = await Task.WhenAny(tcs.Task, Task.Delay(timeoutMs));
      if (completed != tcs.Task)
        throw new TimeoutException("Server không phản hồi.");
      return await tcs.Task;
    }
    finally
    {
      MessageReceived -= Handler;
    }
  }

  public Task SendAsync(NetworkMessage message)
  {
    if (_stream == null)
      throw new InvalidOperationException("Chưa kết nối server.");
    return MessageFramer.SendAsync(_stream, message);
  }

  public async Task<(bool Success, string Message)> RegisterAsync(string username, string password)
  {
    var response = await SendAndWaitAsync(new NetworkMessage
    {
      Type = MessageType.Register,
      Username = username,
      Password = password
    }, MessageType.RegisterResponse);

    return (response.Success, response.Message ?? (response.Success ? "Đăng ký thành công." : "Đăng ký thất bại."));
  }

  public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
  {
    var response = await SendAndWaitAsync(new NetworkMessage
    {
      Type = MessageType.Login,
      Username = username,
      Password = password
    }, MessageType.LoginResponse);

    if (!response.Success)
      return (false, response.Message ?? "Sai tên đăng nhập hoặc mật khẩu.");

    Username = response.Username ?? username;
    PlayerId = response.PlayerId;
    Wins = response.Wins;
    Losses = response.Losses;
    TotalGames = response.TotalGames;
    return (true, response.Message ?? "Đăng nhập thành công.");
  }

  private async Task ReadLoopAsync(CancellationToken ct)
  {
    try
    {
      while (!ct.IsCancellationRequested && _stream != null)
      {
        var message = await MessageFramer.ReadAsync(_stream, _buffer, ct);
        if (message == null)
          break;
        MessageReceived?.Invoke(message);
      }
    }
    catch (Exception) when (ct.IsCancellationRequested)
    {
      // expected on disconnect
    }
    catch (Exception)
    {
      // connection lost
    }
    finally
    {
      Disconnected?.Invoke();
    }
  }

  public void Dispose()
  {
    Disconnect();
  }
}
