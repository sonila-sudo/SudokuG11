using System.Collections.Generic;

namespace Sudoku.Client.Network
{
    // ============================================================
    //  MESSAGE ENVELOPE - cấu trúc chung cho MỌI gói tin
    // ============================================================
    public class Message
    {
        public string Action { get; set; }
        public object Payload { get; set; }
    }

    // ============================================================
    //  C2S - Client gửi lên Server
    // ============================================================

    // 1.1 LOGIN
    public class LoginPayload
    {
        public string PlayerName { get; set; }
    }

    // 1.2 MOVE
    public class MovePayload
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Value { get; set; }
    }

    // 1.3 SURRENDER - Payload rỗng, dùng object là đủ

    // ============================================================
    //  S2C - Server trả về Client
    // ============================================================

    // 2.1 WAITING
    public class WaitingPayload
    {
        public string Message { get; set; }
    }

    // 2.2 MATCH_START
    public class MatchStartPayload
    {
        public string OpponentName { get; set; }
        public List<List<int>> InitialBoard { get; set; }
    }

    // 2.3 MOVE_ACK
    public class MoveAckPayload
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int Value { get; set; }
        public bool IsValid { get; set; }
    }

    // 2.4 TIME_SYNC
    public class TimeSyncPayload
    {
        public string TimeLeft { get; set; }
    }

    // 2.5 GAME_OVER
    public class GameOverPayload
    {
        public string Result { get; set; }   // "WIN" hoặc "LOSE"
        public string Reason { get; set; }
        public string Message { get; set; }
    }
}
