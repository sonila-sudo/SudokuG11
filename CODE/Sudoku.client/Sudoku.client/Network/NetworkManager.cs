using Sudoku.Client.Network;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.client.Network
{
    public class NetworkManager
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public MessageHandler Handler { get; private set; }

        public NetworkManager()
        {
            Handler = new MessageHandler();
        }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();

                _ = Task.Run(() => StartListeningAsync());

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task StartListeningAsync()
        {
            byte[] buffer = new byte[8192];
            while (_client != null && _client.Connected)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Handler.HandleMessage(rawMessage);
                }
                catch
                {
                    break;
                }
            }
        }

        public void SendLogin(string playerName)
        {
            if (_stream == null || !_client.Connected) return;

            try
            {
                // KHÔNG DÙNG CLASS PACKET GỐC NỮA: Tự dựng thủ công chuỗi JSON có định danh $type của Newtonsoft.Json.
                // Chuỗi này mô phỏng chính xác đối tượng LoginPacket thuộc namespace hệ thống của Server.
                // Nó sẽ ép Server bóc tách ra đúng Class Object gốc mà không bị nhận diện thành "Unknown".
                string jsonString = "{\"$type\":\"Sudoku.server.Network.LoginPacket, Sudoku.server\",\"Name\":\"" + playerName + "\",\"Type\":\"LOGIN\"}\n";

                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                _stream.Write(data, 0, data.Length);
                _stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi gửi dữ liệu]: {ex.Message}");
            }
        }
    }
}