using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sudoku.Server.Models
{
    /// <summary>
    /// Thông điệp trao đổi giữa Server và Client qua TCP stream (JSON).
    /// </summary>
    public class GameMessage
    {
        // ── Chung ──────────────────────────────────────────────────────────────────
        [JsonPropertyName("Type")]
        public string Type { get; set; } = string.Empty;

        // BỔ SUNG: Ánh xạ từ khóa "Action" nếu Client gửi bằng trường này
        [JsonPropertyName("Action")]
        public string? Action { get => string.IsNullOrEmpty(Type) ? _action : Type; set { _action = value; if (!string.IsNullOrEmpty(value)) Type = value; } }
        private string? _action;

        [JsonPropertyName("PlayerId")]
        public string? PlayerId { get; set; }

        [JsonPropertyName("RoomId")]
        public string? RoomId { get; set; }

        // ── JOIN / LOGIN ───────────────────────────────────────────────────────────
        [JsonPropertyName("PlayerName")]
        public string? PlayerName { get => _playerName ?? Payload?.PlayerName; set => _playerName = value; }
        private string? _playerName;

        // BỔ SUNG: Khớp với cấu trúc object "Payload" bọc bên ngoài của Client
        [JsonPropertyName("Payload")]
        public GamePayload? Payload { get; set; }

        // ── GAME_START ─────────────────────────────────────────────────────────────
        [JsonPropertyName("Board")]
        public int[][]? Board { get; set; }          // 9x9 puzzle (0 = ô trống)

        [JsonPropertyName("Solution")]
        public int[][]? Solution { get; set; }       // 9x9 đáp án đầy đủ

        // ── MOVE ───────────────────────────────────────────────────────────────────
        [JsonPropertyName("Row")]
        public int Row { get => _row != 0 ? _row : (Payload?.Row ?? 0); set => _row = value; }
        private int _row;

        [JsonPropertyName("Col")]
        public int Col { get => _col != 0 ? _col : (Payload?.Col ?? 0); set => _col = value; }
        private int _col;

        [JsonPropertyName("Value")]
        public int Value { get => _value != 0 ? _value : (Payload?.Value ?? 0); set => _value = value; }
        private int _value;

        // ── MOVE_RESULT ────────────────────────────────────────────────────────────
        [JsonPropertyName("Correct")]
        public bool? Correct { get; set; }

        [JsonPropertyName("ErrorCount")]
        public int ErrorCount { get; set; }

        // ── GAME_OVER ──────────────────────────────────────────────────────────────
        [JsonPropertyName("Winner")]
        public string? Winner { get; set; }          // PlayerId của người thắng

        [JsonPropertyName("WinnerName")]
        public string? WinnerName { get; set; }

        [JsonPropertyName("Reason")]
        public string? Reason { get; set; }          // "COMPLETED" | "3_ERRORS" | "OPPONENT_DISCONNECTED"

        // ── Helpers ────────────────────────────────────────────────────────────────
        private static readonly JsonSerializerOptions _opts = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true // Chấp nhận cả chữ hoa lẫn chữ thường từ Client
        };

        public string Serialize() => JsonSerializer.Serialize(this, _opts);

        public static GameMessage? Deserialize(string json)
        {
            try { return JsonSerializer.Deserialize<GameMessage>(json, _opts); }
            catch { return null; }
        }

        // ── Factory methods ────────────────────────────────────────────────────────
        public static GameMessage MakeGameStart(string roomId, string playerId, int[,] board, int[,] solution) => new()
        {
            Type = "GAME_START",
            RoomId = roomId,
            PlayerId = playerId,
            Board = ToJagged(board),
            Solution = ToJagged(solution)
        };

        public static GameMessage MakeMoveResult(bool correct, int errorCount) => new()
        {
            Type = "MOVE_RESULT",
            Correct = correct,
            ErrorCount = errorCount
        };

        public static GameMessage MakeOpponentMove(int row, int col) => new()
        {
            Type = "OPPONENT_MOVE",
            Row = row,
            Col = col
        };

        public static GameMessage MakeGameOver(string winnerId, string winnerName, string reason) => new()
        {
            Type = "GAME_OVER",
            Winner = winnerId,
            WinnerName = winnerName,
            Reason = reason
        };

        public static GameMessage MakeWaiting() => new() { Type = "WAITING" };
        public static GameMessage MakeError(string msg) => new() { Type = "ERROR", Reason = msg };

        private static int[][] ToJagged(int[,] arr)
        {
            int rows = arr.GetLength(0), cols = arr.GetLength(1);
            var jagged = new int[rows][];
            for (int r = 0; r < rows; r++)
            {
                jagged[r] = new int[cols];
                for (int c = 0; c < cols; c++)
                    jagged[r][c] = arr[r, c];
            }
            return jagged;
        }
    }

    /// <summary>
    /// Lớp bổ trợ để hứng trọn data từ cụm Payload lồng nhau của Client
    /// </summary>
    public class GamePayload
    {
        [JsonPropertyName("PlayerName")]
        public string? PlayerName { get; set; }

        [JsonPropertyName("Row")]
        public int Row { get; set; }

        [JsonPropertyName("Col")]
        public int Col { get; set; }

        [JsonPropertyName("Value")]
        public int Value { get; set; }
    }
}