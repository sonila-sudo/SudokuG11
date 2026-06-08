namespace Sudoku.Server.Models
{
    public enum RoomStatus { Waiting, Playing, Finished }

    /// <summary>
    /// Một phòng đấu Sudoku gồm đúng 2 người chơi.
    /// </summary>
    public class GameRoom
    {
        public string RoomId { get; } = Guid.NewGuid().ToString("N")[..6].ToUpper();
        public Player[] Players { get; } = new Player[2];
        public int[,] Board { get; set; } = new int[9, 9];       // puzzle
        public int[,] Solution { get; set; } = new int[9, 9];    // đáp án
        public RoomStatus Status { get; set; } = RoomStatus.Waiting;
        public DateTime StartTime { get; private set; }

        private readonly object _lock = new object();

        public void AddPlayer(Player player)
        {
            if (Players[0] == null) Players[0] = player;
            else Players[1] = player;
        }

        public bool IsFull => Players[0] != null && Players[1] != null;

        public void StartGame()
        {
            Status = RoomStatus.Playing;
            StartTime = DateTime.UtcNow;
        }

        /// <summary>Trả về đối thủ của một player trong phòng.</summary>
        public Player? GetOpponent(Player player)
        {
            if (Players[0]?.PlayerId == player.PlayerId) return Players[1];
            if (Players[1]?.PlayerId == player.PlayerId) return Players[0];
            return null;
        }

        /// <summary>
        /// Kiểm tra xem board hiện tại của server (đối chiếu solution) đã đầy chưa.
        /// Được gọi mỗi khi player điền đúng một ô.
        /// </summary>
        public bool CheckAllFilled(int[,] playerProgress)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (playerProgress[r, c] == 0) return false;
            return true;
        }

        /// <summary>Lock dùng chung khi cập nhật trạng thái phòng.</summary>
        public object Lock => _lock;

        public override string ToString() => $"[Room {RoomId} {Status}]";
    }
}
