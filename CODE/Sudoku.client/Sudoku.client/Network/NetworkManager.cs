using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Sudoku.Client.Network
{
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected
    }

    /// <summary>
    /// Quản lý toàn bộ kết nối TCP với server.
    /// - ConnectAsync()   : kết nối tới server
    /// - SendLogin()      : gửi gói LOGIN
    /// - SendMove()       : gửi gói MOVE
    /// - SendSurrender()  : gửi gói SURRENDER
    /// - Lắng nghe phản hồi qua MessageHandler events
    /// </summary>
    public class NetworkManager
    {
        // ─── CẤU HÌNH SERVER ──────────────────────────────────────────────
        private const string SERVER_IP   = "127.0.0.1"; // đổi thành IP thật khi cần
        private const int    SERVER_PORT = 9999;         // hỏi bạn làm server để thống nhất

        // ─── THÀNH PHẦN NỘI BỘ ────────────────────────────────────────────
        private TcpClient       _client;
        private NetworkStream   _stream;
        private StreamReader    _reader;

        public ConnectionState  State   { get; private set; } = ConnectionState.Disconnected;
        public MessageHandler   Handler { get; }               = new MessageHandler();

        // ─── SỰ KIỆN LỖI KẾT NỐI ─────────────────────────────────────────
        public event Action<string> OnConnectionError;
        public event Action         OnDisconnected;

        // ─── KẾT NỐI ──────────────────────────────────────────────────────

        /// <summary>
        /// Kết nối bất đồng bộ tới server.
        /// Gọi hàm này khi người dùng bấm "Tìm Trận".
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            try
            {
                State   = ConnectionState.Connecting;
                _client = new TcpClient();

                await _client.ConnectAsync(SERVER_IP, SERVER_PORT);

                _stream = _client.GetStream();
                _reader = new StreamReader(_stream, System.Text.Encoding.UTF8);

                State = ConnectionState.Connected;
                Console.WriteLine("[NetworkManager] Đã kết nối tới server.");

                // Bắt đầu vòng lặp nhận dữ liệu (chạy nền)
                _ = ReceiveLoopAsync();

                return true;
            }
            catch (Exception ex)
            {
                State = ConnectionState.Disconnected;
                OnConnectionError?.Invoke($"Không thể kết nối: {ex.Message}");
                return false;
            }
        }

        // ─── VÒNG LẶP NHẬN ────────────────────────────────────────────────

        /// <summary>
        /// Chạy ngầm, đọc từng dòng JSON từ server và chuyển cho MessageHandler.
        /// </summary>
        private async Task ReceiveLoopAsync()
        {
            try
            {
                while (_client.Connected)
                {
                    string line = await _reader.ReadLineAsync();

                    if (line == null) break; // server đóng kết nối

                    Console.WriteLine($"[NetworkManager] Nhận: {line}");
                    Handler.Handle(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NetworkManager] Mất kết nối: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        // ─── GỬI GÓI TIN ──────────────────────────────────────────────────

        private void Send(string action, object payload)
        {
            if (State != ConnectionState.Connected)
            {
                Console.WriteLine("[NetworkManager] Chưa kết nối, không thể gửi.");
                return;
            }

            try
            {
                byte[] data = PacketSerializer.Serialize(action, payload);
                _stream.Write(data, 0, data.Length);
                Console.WriteLine($"[NetworkManager] Đã gửi: {action}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NetworkManager] Lỗi gửi: {ex.Message}");
                Disconnect();
            }
        }

        /// <summary>Gửi gói LOGIN — gọi khi bấm "Tìm Trận"</summary>
        public void SendLogin(string playerName)
        {
            Send("LOGIN", new LoginPayload { PlayerName = playerName });
        }

        /// <summary>Gửi gói MOVE — gọi khi người chơi điền số vào ô</summary>
        public void SendMove(int row, int col, int value)
        {
            Send("MOVE", new MovePayload { Row = row, Col = col, Value = value });
        }

        /// <summary>Gửi gói SURRENDER — gọi khi bấm "Đầu hàng" hoặc đóng cửa sổ</summary>
        public void SendSurrender()
        {
            Send("SURRENDER", new { });
        }

        // ─── NGẮT KẾT NỐI ─────────────────────────────────────────────────

        public void Disconnect()
        {
            if (State == ConnectionState.Disconnected) return;

            State = ConnectionState.Disconnected;
            _reader?.Close();
            _stream?.Close();
            _client?.Close();

            Console.WriteLine("[NetworkManager] Đã ngắt kết nối.");
            OnDisconnected?.Invoke();
        }
    }
}
