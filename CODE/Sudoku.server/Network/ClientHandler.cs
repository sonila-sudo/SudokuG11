using Sudoku.Server.Models;
using Sudoku.Server.Services;
using System.Net.Sockets;

namespace Sudoku.Server.Network
{
    /// <summary>
    /// Xử lý một kết nối TCP riêng lẻ (chạy trên thread độc lập).
    /// Vòng lặp đọc JSON từ stream → dispatch action → trả lời client.
    /// </summary>
    public class ClientHandler
    {
        private readonly Player _player;
        private readonly RoomManager _roomManager;
        private readonly GameService _gameService;

        public ClientHandler(Player player, RoomManager roomManager, GameService gameService)
        {
            _player = player;
            _roomManager = roomManager;
            _gameService = gameService;
        }

        /// <summary>
        /// Vòng lặp chính – đọc từng dòng JSON từ client.
        /// </summary>
        public void Handle()
        {
            Console.WriteLine($"[Network] {_player} connected from {_player.TcpClient.Client.RemoteEndPoint}");

            using var reader = new StreamReader(_player.Stream);

            try
            {
                while (_player.IsConnected)
                {
                    string? line = reader.ReadLine();
                    if (line == null) break; // client ngắt kết nối

                    GameMessage? msg = MessageSerializer.Deserialize(line);
                    if (msg == null)
                    {
                        _ = _player.SendAsync(GameMessage.MakeError("Invalid JSON"));
                        continue;
                    }

                    Dispatch(msg);
                }
            }
            catch (IOException) { /* client ngắt đột ngột – bình thường */ }
            catch (Exception ex)
            {
                Console.WriteLine($"[Network] {_player} error: {ex.Message}");
            }
            finally
            {
                HandleDisconnect();
            }
        }

        // ── Dispatcher ─────────────────────────────────────────────────────────────

        private void Dispatch(GameMessage msg)
        {
            // Lấy hành động thông qua thuộc tính kết hợp Action/Type đã xử lý mapping
            string? command = (msg.Action ?? msg.Type)?.ToUpper();

            switch (command)
            {
                case "LOGIN":
                case "JOIN":
                    HandleJoin(msg);
                    break;

                case "MOVE":
                    HandleMove(msg);
                    break;

                default:
                    Console.WriteLine($"[Network] Tin nhắn lạ từ {_player.Name}: {command}");
                    _ = _player.SendAsync(GameMessage.MakeError($"Unknown message type: {command}"));
                    break;
            }
        }

        // ── Handlers ───────────────────────────────────────────────────────────────

        private void HandleJoin(GameMessage msg)
        {
            _player.Name = msg.PlayerName ?? "Player";
            Console.WriteLine($"[Network] Người chơi '{_player.Name}' đăng nhập thành công – đang tìm đối thủ...");
            _ = _player.SendAsync(GameMessage.MakeWaiting());
            _roomManager.TryJoin(_player);
        }

        private void HandleMove(GameMessage msg)
        {
            GameRoom? room = _roomManager.GetRoom(msg.RoomId ?? "");
            if (room == null || room.Status != RoomStatus.Playing)
            {
                _ = _player.SendAsync(GameMessage.MakeError("Room not found or game not started"));
                return;
            }
            _gameService.HandleMove(_player, msg, room);
        }

        private void HandleDisconnect()
        {
            Console.WriteLine($"[Network] Người chơi '{_player.Name}' đã ngắt kết nối.");
            _roomManager.HandlePlayerDisconnect(_player);
            try { _player.TcpClient.Close(); } catch { }
        }
    }
}