using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }

        // Khai báo mảng 2 chiều để quản lý 81 ô TextBox sau này dễ truy cập
        private TextBox[,] sudokuCells = new TextBox[9, 9];

        private void GameForm_Load(object sender, EventArgs e)
        {
            CreateSudokuGrid();
        }

        private void CreateSudokuGrid()
{
    int cellSize = 50; // Kích thước mỗi ô vuông

    // 1. Đổi màu nền của Panel thành màu Tối (Màu này sẽ hở ra tạo thành các đường kẻ lưới)
    pnlBoard.BackColor = Color.Navy; 

    for (int row = 0; row < 9; row++)
    {
        for (int col = 0; col < 9; col++)
        {
            TextBox txt = new TextBox();
            txt.Size = new Size(cellSize, cellSize);
            
            // --- 2. TOÁN HỌC ĐỂ TẠO KHOẢNG CÁCH (ĐƯỜNG KẺ LƯỚI) ---
            // Thêm 1 pixel khoảng cách cho các ô bình thường (lưới mỏng)
            int thinLineX = col * 1; 
            int thinLineY = row * 1;
            
            // Thêm 3 pixel khoảng cách sau mỗi 3 ô (lưới đậm chia khối 3x3)
            int thickLineX = (col / 3) * 3; 
            int thickLineY = (row / 3) * 3;

            txt.Location = new Point(
                (col * cellSize) + thinLineX + thickLineX, 
                (row * cellSize) + thinLineY + thickLineY
            );

            txt.Multiline = true;
            txt.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            txt.ForeColor = Color.DarkSlateGray; 
            txt.TextAlign = HorizontalAlignment.Center;
            txt.MaxLength = 1;
            
            // Tắt viền 3D mặc định của TextBox để giao diện phẳng
            txt.BorderStyle = BorderStyle.None; 

            // --- 3. TÔ MÀU NỀN XEN KẼ CHO CÁC KHỐI 3x3 GIỐNG ẢNH ---
            // Công thức xác định xem ô này đang nằm ở khối 3x3 chẵn hay lẻ
            bool isAlternateBlock = ((row / 3) + (col / 3)) % 2 != 0;
            if (isAlternateBlock)
            {
                txt.BackColor = Color.AliceBlue; // Màu xanh nhạt
            }
            else
            {
                txt.BackColor = Color.White; // Màu trắng
            }

            // Lưu vào mảng 2 chiều để quản lý
            sudokuCells[row, col] = txt;

            // Thêm ô này vào trong Panel
            pnlBoard.Controls.Add(txt);
        }
    }
}

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pnlBoard_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
