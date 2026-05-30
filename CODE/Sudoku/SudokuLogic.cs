using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku
{
    public class SudokuLogic
    {
        //Mảng 2 chiều lưu giá trị bàn cờ
        public int[,] board = new int[9, 9];
        private Random random = new Random();

        //Kiểm tra tính hợp lệ
        public bool CheckValid(int row, int col, int num)
        {
            if (num == 0) return true; //Nếu số được đặt ô trống thì luôn hợp lệ
            for (int i = 0; i < 9; i++)
            {
                //Kiểm tra hàng và cột , bỏ qua ô đang xét
                if (i != col && board[row, i] == num) return false;
                if (i != row && board[i, col] == num) return false;
            }
            //kiểm tra khối 3x3
            int sR = (row / 3) * 3, sC = (col / 3) * 3;
            for (int i = sR; i < sR + 3; i++)
                for (int j = sC; j < sC + 3; j++)
                    if ((i != row || j != col) && board[i, j] == num) return false;
            return true;
        }
        //Sinh đề bài: tạo bàn cờ ngẫu nhiên rồi xóa đi một số ô
        public void GenerateRandomBoard(int emptyCells)
        {
            Array.Clear(board, 0, board.Length);
            //Lắp đầy bàn cờ ngẫu nhiên ( đảm bảo luôn có lời giải )

            Fill(0, 0);
            //Xóa ngẫu nhiên các ô dựa trên tham số 'emptyCells' để tạo độ khó
            int rem = 0;
            while (rem < emptyCells)
            {
                int r = random.Next(9), c = random.Next(9);
                if (board[r, c] != 0) { board[r, c] = 0; rem++; }
            }
        }
        // (Backtracking): Thử từng số từ 1-9 vào từng ô

        private bool Fill(int r, int c)
        {
            // Nếu đã đi hết cột 9, nhảy xuống hàng tiếp theo
            if (c == 9) { r++; c = 0; }
            // Nếu đã đi đến hàng thứ 9, nghĩa là bàn cờ đã lấp đầy thành công
            if (r == 9) return true;
            // Tạo danh sách 1-9 và xáo trộn ngẫu nhiên để mỗi lần chơi là 1 bàn cờ khác nhau
            var nums = Enumerable.Range(1, 9).OrderBy(x => random.Next()).ToList();
            foreach (var n in nums)
            {
                if (CheckValid(r, c, n))// Nếu số n đặt vào được
                {
                    board[r, c] = n;// Đặt thử số n vào bàn cờ
                    if (Fill(r, c + 1)) return true;// Gọi đệ quy để điền ô tiếp theo (c + 1)
                    board[r, c] = 0;// Nếu đi tiếp mà bị "bí", quay lui (Backtracking): gán lại bằng 0 để thử số khác
                }
            }
            return false;// Không tìm được số nào phù hợp cho ô này
        }
    }
}