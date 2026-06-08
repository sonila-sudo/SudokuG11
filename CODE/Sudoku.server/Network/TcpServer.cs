using Sudoku.Server.Models;
using Sudoku.Server.Services;
using System.Net;
using System.Net.Sockets;

namespace Sudoku.Server.Network
{
    /// <summary>
    /// TCP Listener chính – lắng nghe kết nối và spawn Thread per client.
    /// </summary>
    public class TcpServer
    {
        private readonly int _port;
        private readonly RoomManager _roomManager;
        private readonly GameService _gameService;

        private TcpListener? _listener;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public TcpServer(int port, RoomManager roomManager, GameService gameService)
        {
            _port = port;
            _roomManager = roomManager;
            _gameService = gameService;
        }

        /// <summary>
        /// Bắt đầu lắng nghe (blocking async loop).
        /// Gọi trên main thread – vòng lặp chạy cho đến khi Cancel.
        /// </summary>
        public async Task StartAsync()
        {
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"[Server] Listening on 0.0.0.0:{_port} ...");
            Console.WriteLine("[Server] Waiting for players to connect...\n");

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync(_cts.Token);
                    OnClientAccepted(tcpClient);
                }
            }
            catch (OperationCanceledException) { /* Stop() được gọi */ }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Fatal error: {ex.Message}");
            }
            finally
            {
                _listener.Stop();
                Console.WriteLine("[Server] Stopped.");
            }
        }

        /// <summary>Dừng server.</summary>
        public void Stop()
        {
            _cts.Cancel();
        }

        // ── Private ────────────────────────────────────────────────────────────────

        private void OnClientAccepted(TcpClient tcpClient)
        {
            var player = new Player(tcpClient);
            var handler = new ClientHandler(player, _roomManager, _gameService);

            // Thread-per-client: mỗi client có luồng riêng để đọc blocking
            Thread thread = new Thread(() => handler.Handle())
            {
                IsBackground = true,
                Name = $"Client-{player.PlayerId}"
            };
            thread.Start();

            Console.WriteLine($"[Server] New connection accepted → {thread.Name}");
        }
    }
}
