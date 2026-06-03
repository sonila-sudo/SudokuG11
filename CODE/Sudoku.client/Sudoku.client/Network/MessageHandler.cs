using System;

namespace Sudoku.Client.Network
{
    /// <summary>
    /// Nhận chuỗi JSON thô từ NetworkManager,
    /// đọc Action rồi bắn đúng event để GUI / GameLogic lắng nghe.
    /// </summary>
    public class MessageHandler
    {
        // ─── EVENTS (GUI và GameLogic sẽ đăng ký lắng nghe) ───────────────

        public event Action<WaitingPayload>    OnWaiting;
        public event Action<MatchStartPayload> OnMatchStart;
        public event Action<MoveAckPayload>    OnMoveAck;
        public event Action<TimeSyncPayload>   OnTimeSync;
        public event Action<GameOverPayload>   OnGameOver;

        // ─── XỬ LÝ GÓI TIN ────────────────────────────────────────────────

        /// <summary>
        /// Gọi hàm này mỗi khi nhận được 1 dòng JSON từ server.
        /// </summary>
        public void Handle(string json)
        {
            try
            {
                string action = PacketSerializer.GetAction(json);

                switch (action)
                {
                    case "WAITING":
                        OnWaiting?.Invoke(
                            PacketSerializer.DeserializePayload<WaitingPayload>(json));
                        break;

                    case "MATCH_START":
                        OnMatchStart?.Invoke(
                            PacketSerializer.DeserializePayload<MatchStartPayload>(json));
                        break;

                    case "MOVE_ACK":
                        OnMoveAck?.Invoke(
                            PacketSerializer.DeserializePayload<MoveAckPayload>(json));
                        break;

                    case "TIME_SYNC":
                        OnTimeSync?.Invoke(
                            PacketSerializer.DeserializePayload<TimeSyncPayload>(json));
                        break;

                    case "GAME_OVER":
                        OnGameOver?.Invoke(
                            PacketSerializer.DeserializePayload<GameOverPayload>(json));
                        break;

                    default:
                        Console.WriteLine($"[MessageHandler] Unknown action: {action}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MessageHandler] Error handling message: {ex.Message}");
            }
        }
    }
}
