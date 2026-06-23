using Sudoku.Server.Models;

namespace Sudoku.Server.Services
{
    /// <summary>
    /// Quản lý matchmaking: ghép 2 player vào một GameRoom và xử lý ngắt kết nối.
    /// Thread-safe nhờ lock trên _waitingLock.
    /// GameService được set qua property để tránh circular dependency.
    /// </summary>
    public class RoomManager
    {
        // Set bởi Program.cs sau khi cả 2 đều khởi tạo xong
        public GameService GameService { get; set; } = null!;

        private Player? _waitingPlayer = null;
        private readonly object _waitingLock = new object();

        private readonly Dictionary<string, GameRoom> _rooms = new();
        private readonly object _roomsLock = new object();

        public RoomManager() { }

        /// <summary>
        /// Thử ghép player vào phòng.
        /// </summary>
        public void TryJoin(Player newPlayer)
        {
            GameRoom? roomToStart = null;

            lock (_waitingLock)
            {
                if (_waitingPlayer == null)
                {
                    _waitingPlayer = newPlayer;
                    Console.WriteLine($"[Room] {newPlayer.Name} đang ở hàng đợi, chờ đối thủ...");
                }
                else
                {
                    // Ghép phòng khi xuất hiện người chơi thứ 2
                    var room = new GameRoom();
                    room.AddPlayer(_waitingPlayer);
                    room.AddPlayer(newPlayer);

                    lock (_roomsLock)
                        _rooms[room.RoomId] = room;

                    Console.WriteLine($"[Room] Trận đấu khởi tạo thành công: {_waitingPlayer.Name} VS {newPlayer.Name}");
                    _waitingPlayer = null;
                    roomToStart = room;
                }
            }

            // Khởi động game bên ngoài lock để tránh deadlock khi gửi network
            if (roomToStart != null)
                GameService.StartGame(roomToStart);
        }

        /// <summary>Lấy phòng theo ID.</summary>
        public GameRoom? GetRoom(string roomId)
        {
            lock (_roomsLock)
                return _rooms.TryGetValue(roomId, out var room) ? room : null;
        }

        /// <summary>
        /// Xử lý khi một player mất kết nối đột ngột.
        /// </summary>
        public void HandlePlayerDisconnect(Player player)
        {
            lock (_waitingLock)
            {
                if (_waitingPlayer?.PlayerId == player.PlayerId)
                {
                    _waitingPlayer = null;
                    Console.WriteLine($"[Room] Người chơi đang chờ {player.Name} đã hủy tìm trận.");
                    return;
                }
            }

            GameRoom? room = null;
            lock (_roomsLock)
            {
                room = _rooms.Values.FirstOrDefault(r =>
                    r.Players[0]?.PlayerId == player.PlayerId ||
                    r.Players[1]?.PlayerId == player.PlayerId);
            }

            if (room == null || room.Status == RoomStatus.Finished) return;

            lock (room.Lock)
            {
                if (room.Status == RoomStatus.Finished) return;
                room.Status = RoomStatus.Finished;
            }

            Player? opponent = room.GetOpponent(player);
            if (opponent != null)
            {
                Console.WriteLine($"[Room] {player.Name} mất kết nối đột ngột – {opponent.Name} thắng cuộc.");
                _ = opponent.SendAsync(GameMessage.MakeGameOver(
                    opponent.PlayerId,
                    opponent.Name,
                    "OPPONENT_DISCONNECTED"
                ));
            }
        }

        /// <summary>Đóng một phòng và xóa khỏi danh sách.</summary>
        public void CloseRoom(GameRoom room)
        {
            lock (_roomsLock)
                _rooms.Remove(room.RoomId);
        }
    }
}