using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.client
{
    public class SudokuLogic
    {
        public int[,] board = new int[9, 9];
        public int[,] solutionBoard = new int[9, 9];
        private Random random = new Random();

        public bool CheckValid(int row, int col, int num)
        {
            if (num == 0) return true;
            for (int i = 0; i < 9; i++)
            {
                if (i != col && board[row, i] == num) return false;
                if (i != row && board[i, col] == num) return false;
            }
            int sR = (row / 3) * 3, sC = (col / 3) * 3;
            for (int i = sR; i < sR + 3; i++)
                for (int j = sC; j < sC + 3; j++)
                    if ((i != row || j != col) && board[i, j] == num) return false;
            return true;
        }

        public void GenerateRandomBoard(int emptyCells)
        {
            Array.Clear(board, 0, board.Length);
            Fill(0, 0);

            Array.Copy(board, solutionBoard, board.Length);

            List<(int row, int col)> cells = new List<(int row, int col)>();
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    cells.Add((r, c));

            cells = cells.OrderBy(x => random.Next()).ToList();

            int removed = 0;
            foreach (var cell in cells)
            {
                if (removed >= emptyCells) break;
                int backup = board[cell.row, cell.col];
                board[cell.row, cell.col] = 0;

                if (CountSolutions((int[,])board.Clone()) == 1) removed++;
                else board[cell.row, cell.col] = backup;
            }
        }

        private bool Fill(int r, int c)
        {
            if (c == 9) { r++; c = 0; }
            if (r == 9) return true;
            var nums = Enumerable.Range(1, 9).OrderBy(x => random.Next()).ToList();
            foreach (var n in nums)
            {
                if (CheckValid(r, c, n))
                {
                    board[r, c] = n;
                    if (Fill(r, c + 1)) return true;
                    board[r, c] = 0;
                }
            }
            return false;
        }

        private int CountSolutions(int[,] tempBoard, int limit = 2)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (tempBoard[r, c] == 0)
                    {
                        int count = 0;
                        for (int n = 1; n <= 9; n++)
                        {
                            if (IsValidForBoard(tempBoard, r, c, n))
                            {
                                tempBoard[r, c] = n;
                                count += CountSolutions(tempBoard, limit);
                                tempBoard[r, c] = 0;
                                if (count >= limit) return count;
                            }
                        }
                        return count;
                    }
            return 1;
        }

        private bool IsValidForBoard(int[,] b, int r, int c, int n)
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