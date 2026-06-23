using System;

namespace Sudoku.Client.Network
{
    public class MessageHandler
    {
        // Nhận vào một chuỗi thông điệp tổng quát để kích hoạt chuyển Form
        public event Action<string> OnMatchStart;

        public void HandleMessage(string rawMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawMessage)) return;

                string cleanMessage = rawMessage.Trim().ToUpper();

                // Lắng nghe tất cả các từ khóa bắt đầu game từ Server của bạn gửi về
                if (cleanMessage.Contains("GAME_START") || cleanMessage.Contains("MATCH_START") || cleanMessage.Contains("PLAYING"))
                {
                    OnMatchStart?.Invoke(rawMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Lỗi đọc tin nhắn mạng]: {ex.Message}");
            }
        }
    }
}