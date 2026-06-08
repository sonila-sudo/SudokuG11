using Sudoku.Server.Models;

namespace Sudoku.Server.Services
{
    /// <summary>
    /// Xử lý toàn bộ logic game: phát board, validate move, tính thắng thua.
    /// Luật:
    ///   - Sai 3 lỗi → thua (và đối thủ thắng)
    ///   - Điền đúng toàn bộ board → thắng nếu thời gian nhỏ hơn đối thủ
    /// </summary>
    public class GameService
    {
        private readonly SudokuGenerator _generator;
        // Track progress của từng player: playerId → board đã điền
        private readonly Dictionary<string, int[,]> _playerBoards = new();
        private readonly object _boardsLock = new object();

        public GameService(SudokuGenerator generator)
        {
            _generator = generator;
        }

        // ── StartGame ──────────────────────────────────────────────────────────────

        public void StartGame(GameRoom room)
        {
            // Tạo puzzle và solution
            var (puzzle, solution) = _generator.Generate(emptyCells: 40);
            room.Board = puzzle;
            room.Solution = solution;
            room.StartGame();

            // Khởi tạo board tiến trình của mỗi player (copy từ puzzle)
            lock (_boardsLock)
            {
                foreach (var p in room.Players)
                    _playerBoards[p.PlayerId] = (int[,])puzzle.Clone();
            }

            Console.WriteLine($"[Game] {room} started – distributing board to players");

            // Gửi GAME_START cho từng player (với PlayerId của chính họ)
            foreach (var player in room.Players)
            {
                _ = player.SendAsync(GameMessage.MakeGameStart(
                    room.RoomId,
                    player.PlayerId,
                    puzzle,
                    solution
                ));
                Console.WriteLine($"[Game] → GAME_START sent to {player}");
            }
        }

        // ── HandleMove ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Xử lý một nước đi của player.
        /// Validate với solution, cập nhật trạng thái, phát kết quả.
        /// </summary>
        public void HandleMove(Player player, GameMessage msg, GameRoom room)
        {
            // Bảo vệ: không xử lý nếu game kết thúc hoặc player đã bị loại
            if (room.Status == RoomStatus.Finished) return;
            if (player.IsEliminated || player.IsFinished) return;

            int row = msg.Row;
            int col = msg.Col;
            int value = msg.Value;

            // Kiểm tra tọa độ hợp lệ
            if (row < 0 || row > 8 || col < 0 || col > 8 || value < 1 || value > 9)
            {
                _ = player.SendAsync(GameMessage.MakeError("Invalid move coordinates"));
                return;
            }

            bool correct = (room.Solution[row, col] == value);

            if (correct)
            {
                // Cập nhật board tiến trình của player này
                int[,] progress;
                lock (_boardsLock)
                {
                    progress = _playerBoards[player.PlayerId];
                    progress[row, col] = value;
                }

                // Gửi kết quả đúng cho player
                _ = player.SendAsync(GameMessage.MakeMoveResult(true, player.ErrorCount));

                // Thông báo cho đối thủ biết vị trí đó đã được điền
                Player? opponent = room.GetOpponent(player);
                if (opponent != null && !opponent.IsEliminated)
                    _ = opponent.SendAsync(GameMessage.MakeOpponentMove(row, col));

                Console.WriteLine($"[Game] {player} CORRECT move ({row},{col})={value}");

                // Kiểm tra hoàn thành board
                bool allFilled = IsBoardComplete(progress, room.Solution);
                if (allFilled)
                    HandlePlayerFinished(player, room);
            }
            else
            {
                // Tăng lỗi
                bool eliminated = player.AddError();
                _ = player.SendAsync(GameMessage.MakeMoveResult(false, player.ErrorCount));

                Console.WriteLine($"[Game] {player} WRONG move ({row},{col})={value} → errors={player.ErrorCount}");

                if (eliminated)
                    HandleElimination(player, room);
            }
        }

        // ── Private Outcome Handlers ───────────────────────────────────────────────

        /// <summary>Xử lý khi player điền đúng toàn bộ board.</summary>
        private void HandlePlayerFinished(Player player, GameRoom room)
        {
            player.MarkFinished();
            Console.WriteLine($"[Game] {player} FINISHED the board!");

            Player? opponent = room.GetOpponent(player);

            // Nếu đối thủ đã bị loại trước (3 lỗi) → player này thắng ngay
            if (opponent == null || opponent.IsEliminated)
            {
                DeclareWinner(player, room, "COMPLETED");
                return;
            }

            // Nếu đối thủ chưa xong → chờ
            if (!opponent.IsFinished)
            {
                // Thông báo cho player biết họ xong trước, chờ kết quả
                _ = player.SendAsync(new Models.GameMessage
                {
                    Type = "WAITING_RESULT",
                    Reason = "You finished! Waiting for time comparison..."
                });
                Console.WriteLine($"[Game] {player} waiting for opponent to finish (time race)...");
                return;
            }

            // Cả 2 đều hoàn thành → so sánh thời gian (ai nhỏ hơn thắng)
            CompareAndDeclareWinner(player, opponent, room);
        }

        /// <summary>So sánh thời gian hoàn thành và tuyên bố người thắng.</summary>
        private void CompareAndDeclareWinner(Player p1, Player p2, GameRoom room)
        {
            lock (room.Lock)
            {
                if (room.Status == RoomStatus.Finished) return;
                room.Status = RoomStatus.Finished;
            }

            Player winner = (p1.FinishTime <= p2.FinishTime) ? p1 : p2;
            Player loser = (winner == p1) ? p2 : p1;

            double winSecs = (winner.FinishTime!.Value - room.StartTime).TotalSeconds;
            double loseSecs = (loser.FinishTime!.Value - room.StartTime).TotalSeconds;

            Console.WriteLine($"[Game] {room} TIME RACE – {winner} {winSecs:F1}s vs {loser} {loseSecs:F1}s → {winner} WINS");

            var msg = GameMessage.MakeGameOver(winner.PlayerId, winner.Name, "COMPLETED");
            _ = winner.SendAsync(msg);
            _ = loser.SendAsync(msg);
        }

        /// <summary>Tuyên bố winner trực tiếp (không cần so sánh thời gian).</summary>
        private void DeclareWinner(Player winner, GameRoom room, string reason)
        {
            lock (room.Lock)
            {
                if (room.Status == RoomStatus.Finished) return;
                room.Status = RoomStatus.Finished;
            }

            Console.WriteLine($"[Game] {room} → {winner} WINS ({reason})");
            var msg = GameMessage.MakeGameOver(winner.PlayerId, winner.Name, reason);

            foreach (var p in room.Players)
                _ = p.SendAsync(msg);
        }

        /// <summary>Xử lý khi player bị loại do 3 lỗi.</summary>
        private void HandleElimination(Player player, GameRoom room)
        {
            Console.WriteLine($"[Game] {player} ELIMINATED (3 errors)");

            lock (room.Lock)
            {
                if (room.Status == RoomStatus.Finished) return;
                room.Status = RoomStatus.Finished;
            }

            Player? opponent = room.GetOpponent(player);

            // Gửi thua cho player bị loại
            _ = player.SendAsync(GameMessage.MakeGameOver(
                opponent?.PlayerId ?? "",
                opponent?.Name ?? "Opponent",
                "3_ERRORS"
            ));

            // Gửi thắng cho đối thủ
            if (opponent != null)
            {
                _ = opponent.SendAsync(GameMessage.MakeGameOver(
                    opponent.PlayerId,
                    opponent.Name,
                    "3_ERRORS"
                ));
            }
        }

        // ── Util ───────────────────────────────────────────────────────────────────

        private static bool IsBoardComplete(int[,] progress, int[,] solution)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (progress[r, c] != solution[r, c]) return false;
            return true;
        }
    }
}
