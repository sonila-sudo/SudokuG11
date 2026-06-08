using System.Net.Sockets;

namespace Sudoku.Server.Models
{
    /// <summary>
    /// Đại diện cho một người chơi đang kết nối tới server.
    /// </summary>
    public class Player
    {
        public string PlayerId { get; } = Guid.NewGuid().ToString("N")[..8];
        public string Name { get; set; } = "Unknown";

        // ── Kết nối ────────────────────────────────────────────────────────────────
        public TcpClient TcpClient { get; set; }
        public NetworkStream Stream => TcpClient.GetStream();
        public bool IsConnected => TcpClient?.Connected ?? false;

        // ── Trạng thái game ────────────────────────────────────────────────────────
        public int ErrorCount { get; private set; } = 0;
        public DateTime? FinishTime { get; private set; } = null;
        public bool IsFinished => FinishTime.HasValue;
        public bool IsEliminated => ErrorCount >= 3;

        // ── Lock để ghi stream an toàn đa luồng ───────────────────────────────────
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        public Player(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }

        /// <summary>Ghi một thông điệp JSON xuống stream (thread-safe).</summary>
        public async Task SendAsync(GameMessage message)
        {
            if (!IsConnected) return;
            try
            {
                await _writeLock.WaitAsync();
                string json = message.Serialize();
                var writer = new StreamWriter(Stream) { AutoFlush = true };
                await writer.WriteLineAsync(json);
            }
            catch { /* client đã ngắt kết nối */ }
            finally
            {
                _writeLock.Release();
            }
        }

        /// <summary>Tăng số lỗi. Trả về true nếu đã đủ 3 lỗi (bị loại).</summary>
        public bool AddError()
        {
            ErrorCount++;
            return IsEliminated;
        }

        /// <summary>Đánh dấu người chơi đã hoàn thành board.</summary>
        public void MarkFinished() => FinishTime = DateTime.UtcNow;

        public override string ToString() => $"[Player {PlayerId} \"{Name}\"]";
    }
}
