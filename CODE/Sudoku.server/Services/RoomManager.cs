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
        /// - Nếu chưa có ai chờ → player này chờ.
        /// - Nếu đã có người chờ → tạo phòng và bắt đầu game.
        /// </summary>
        public void TryJoin(Player newPlayer)
        {
            GameRoom? roomToStart = null;

            lock (_waitingLock)
            {
                if (_waitingPlayer == null)
                {
                    _waitingPlayer = newPlayer;
                    Console.WriteLine($"[Room] {newPlayer} is waiting for an opponent...");
                }
                else
                {
                    // Ghép phòng
                    var room = new GameRoom();
                    room.AddPlayer(_waitingPlayer);
                    room.AddPlayer(newPlayer);

                    lock (_roomsLock)
                        _rooms[room.RoomId] = room;

                    _waitingPlayer = null;
                    roomToStart = room;
                    Console.WriteLine($"[Room] {room} created – {room.Players[0]} vs {room.Players[1]}");
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
        /// Thông báo cho đối thủ nếu game đang diễn ra.
        /// </summary>
        public void HandlePlayerDisconnect(Player player)
        {
            // Nếu player đang chờ → xóa khỏi hàng chờ
            lock (_waitingLock)
            {
                if (_waitingPlayer?.PlayerId == player.PlayerId)
                {
                    _waitingPlayer = null;
                    Console.WriteLine($"[Room] Waiting player {player} disconnected.");
                    return;
                }
            }

            // Nếu đang trong phòng → tìm phòng và thông báo đối thủ
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
                Console.WriteLine($"[Room] {player} disconnected from {room} – {opponent} wins by default.");
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
