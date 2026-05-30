using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class GameForm : Form
    {
        private SudokuLogic logic = new SudokuLogic();
        private TextBox[,] sudokuCells = new TextBox[9, 9];

        
        Color colorDefaultBack = Color.White;
        Color colorAltBack = Color.AliceBlue;
        Color colorHighlightLine = Color.FromArgb(232, 239, 247); // Xanh cực nhạt cho đường chặn
        Color colorSameNumber = Color.FromArgb(205, 220, 240);   // Xanh vừa cho số giống nhau
        Color colorFocus = Color.FromArgb(180, 210, 245);        // Ô đang chọn

        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            CreateSudokuGrid();
            StartNewGame(40); // Tạo đề bài 40 ô trống
        }

        private void CreateSudokuGrid()
        {
            int cellSize = 50;
            int borderThickness = 3;

            pnlBoard.BackColor = Color.Navy;
            pnlBoard.Size = new Size(470, 470);
            pnlBoard.Controls.Clear();

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox txt = new TextBox();
                    txt.Size = new Size(cellSize, cellSize);
                    txt.Multiline = true;
                    txt.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                    txt.ForeColor = Color.DarkSlateGray;
                    txt.TextAlign = HorizontalAlignment.Center;
                    txt.MaxLength = 1;
                    txt.BorderStyle = BorderStyle.None;

                    
                    int thinLineX = col * 1;
                    int thinLineY = row * 1;
                    int thickLineX = (col / 3) * 3;
                    int thickLineY = (row / 3) * 3;

                    txt.Location = new Point(
                        (col * cellSize) + thinLineX + thickLineX + borderThickness,
                        (row * cellSize) + thinLineY + thickLineY + borderThickness
                    );

                    bool isAlternateBlock = ((row / 3) + (col / 3)) % 2 != 0;
                    txt.BackColor = isAlternateBlock ? colorAltBack : colorDefaultBack;

                    // Gán dữ liệu tọa độ
                    txt.Tag = new int[] { row, col };

                    // Sự kiện Click/Tab vào ô để Highlight
                    txt.Enter += (s, ev) => {
                        int[] pos = (int[])((TextBox)s).Tag;
                        ApplyHighlight(pos[0], pos[1]);
                    };

                    txt.KeyPress += Txt_KeyPress;
                    txt.TextChanged += Cell_TextChanged;

                    sudokuCells[row, col] = txt;
                    pnlBoard.Controls.Add(txt);


                    txt.Click += (s, ev) => {
                        ((TextBox)s).SelectAll(); // Nhấn vào là bôi đen để gõ đè số mới ngay
                    };

                    txt.Enter += (s, ev) => {
                        TextBox currentTxt = (TextBox)s;
                        currentTxt.SelectAll(); // Di chuyển bằng phím Tab cũng bôi đen luôn

                        // Giữ nguyên logic Highlight khi chọn ô
                        int[] pos = (int[])currentTxt.Tag;
                        ApplyHighlight(pos[0], pos[1]);
                    };
                }
            }
        }

        private void ApplyHighlight(int row, int col)
        {
            string targetValue = sudokuCells[row, col].Text;

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    bool isAlt = ((r / 3) + (c / 3)) % 2 != 0;

                    // Highlight đường chặn
                    if (r == row || c == col)
                        sudokuCells[r, c].BackColor = colorHighlightLine;
                    else
                        sudokuCells[r, c].BackColor = isAlt ? colorAltBack : colorDefaultBack;

                    // Highlight số giống nhau
                    if (!string.IsNullOrEmpty(targetValue) && sudokuCells[r, c].Text == targetValue)
                        sudokuCells[r, c].BackColor = colorSameNumber;
                }
            }
            // Ô đang chọn
            sudokuCells[row, col].BackColor = colorFocus;
        }

        private void Cell_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null || txt.Tag == null) return;
            int[] pos = (int[])txt.Tag;
            int r = pos[0], c = pos[1];

            if (int.TryParse(txt.Text, out int num))
            {
                logic.board[r, c] = num;
                // Kiểm tra đúng/sai để đổi màu chữ
                txt.ForeColor = logic.CheckValid(r, c, num) ? Color.Blue : Color.Red;
                ApplyHighlight(r, c); // Highlight các số giống nhau vừa nhập
            }
            else
            {
                // Khi xóa số (ô trống)
                logic.board[r, c] = 0;
                ApplyHighlight(r, c); //  Để reset màu highlight quanh ô trống đó
            }
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Chỉ cho nhập số 1-9 và phím xóa
            if (!char.IsDigit(e.KeyChar) || e.KeyChar == '0')
                if (e.KeyChar != (char)8) e.Handled = true;
        }

        public void StartNewGame(int emptyCells)
        {
            logic.GenerateRandomBoard(emptyCells);
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    int val = logic.board[r, c];
                    sudokuCells[r, c].TextChanged -= Cell_TextChanged;
                    if (val != 0)
                    {
                        sudokuCells[r, c].Text = val.ToString();
                        sudokuCells[r, c].ReadOnly = true;
                        sudokuCells[r, c].ForeColor = Color.Black;
                    }
                    else
                    {
                        sudokuCells[r, c].Text = "";
                        sudokuCells[r, c].ReadOnly = false;
                        sudokuCells[r, c].ForeColor = Color.Blue;
                    }
                    sudokuCells[r, c].TextChanged += Cell_TextChanged;

                    bool isAlt = ((r / 3) + (c / 3)) % 2 != 0;
                    sudokuCells[r, c].BackColor = isAlt ? colorAltBack : colorDefaultBack;
                }
        }

        
        private void label1_Click(object sender, EventArgs e) { }
        private void pnlBoard_Paint(object sender, PaintEventArgs e) { }
        private void lblGameStatus_Click(object sender, EventArgs e) { }
    }
}