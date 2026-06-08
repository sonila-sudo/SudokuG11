namespace Sudoku.Server.Services
{
    /// <summary>
    /// Tạo board Sudoku ngẫu nhiên có đúng 1 nghiệm, độc lập với client.
    /// Logic được port từ SudokuLogic.cs của project Client.
    /// </summary>
    public class SudokuGenerator
    {
        private int[,] _board = new int[9, 9];
        private int[,] _solution = new int[9, 9];
        private readonly Random _rng = new Random();

        /// <summary>
        /// Tạo board mới với số ô trống cho trước.
        /// Trả về (puzzle, solution).
        /// </summary>
        public (int[,] puzzle, int[,] solution) Generate(int emptyCells = 40)
        {
            _board = new int[9, 9];
            Fill(0, 0);

            // Lưu đáp án đầy đủ
            _solution = (int[,])_board.Clone();

            // Xóa bớt ô (giữ đúng 1 nghiệm)
            var cells = new List<(int r, int c)>();
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    cells.Add((r, c));

            cells = cells.OrderBy(_ => _rng.Next()).ToList();

            int removed = 0;
            foreach (var (r, c) in cells)
            {
                if (removed >= emptyCells) break;
                int backup = _board[r, c];
                _board[r, c] = 0;

                if (CountSolutions((int[,])_board.Clone()) == 1)
                    removed++;
                else
                    _board[r, c] = backup;
            }

            return ((int[,])_board.Clone(), (int[,])_solution.Clone());
        }

        // ── Private ────────────────────────────────────────────────────────────────

        private bool Fill(int r, int c)
        {
            if (c == 9) { r++; c = 0; }
            if (r == 9) return true;

            var nums = Enumerable.Range(1, 9).OrderBy(_ => _rng.Next()).ToList();
            foreach (int n in nums)
            {
                if (IsValid(_board, r, c, n))
                {
                    _board[r, c] = n;
                    if (Fill(r, c + 1)) return true;
                    _board[r, c] = 0;
                }
            }
            return false;
        }

        private int CountSolutions(int[,] b, int limit = 2)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (b[r, c] == 0)
                    {
                        int count = 0;
                        for (int n = 1; n <= 9; n++)
                        {
                            if (IsValid(b, r, c, n))
                            {
                                b[r, c] = n;
                                count += CountSolutions(b, limit);
                                b[r, c] = 0;
                                if (count >= limit) return count;
                            }
                        }
                        return count;
                    }
            return 1;
        }

        private static bool IsValid(int[,] b, int r, int c, int n)
        {
            for (int i = 0; i < 9; i++)
                if (b[r, i] == n || b[i, c] == n) return false;
            int sR = (r / 3) * 3, sC = (c / 3) * 3;
            for (int i = sR; i < sR + 3; i++)
                for (int j = sC; j < sC + 3; j++)
                    if (b[i, j] == n) return false;
            return true;
        }
    }
}
