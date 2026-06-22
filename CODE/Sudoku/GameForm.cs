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
        Color colorHighlightLine = Color.FromArgb(232, 239, 247);
        Color colorSameNumber = Color.FromArgb(205, 220, 240);
        Color colorFocus = Color.FromArgb(180, 210, 245);

        public GameForm()
        {
            InitializeComponent();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            CreateSudokuGrid();
            StartNewGame(40);
        }

        private void CreateSudokuGrid()
        {
            pnlBoard.Controls.Clear();
            pnlBoard.BackColor = Color.Navy;

            int boardWidth = pnlBoard.ClientSize.Width;
            int boardHeight = pnlBoard.ClientSize.Height;

            int margin = 4;

            double availableW = boardWidth - (margin * 2) - 4;
            double availableH = boardHeight - (margin * 2) - 4;

            double cellW = availableW / 9;
            double cellH = availableH / 9;

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox txt = new TextBox();
                    txt.Multiline = true;
                    txt.Font = new Font("Segoe UI", 16, FontStyle.Bold);
                    txt.ForeColor = Color.DarkSlateGray;
                    txt.TextAlign = HorizontalAlignment.Center;
                    txt.MaxLength = 1;
                    txt.BorderStyle = BorderStyle.None;

                    int thickLinesX = (col / 3) * 2;
                    int thickLinesY = (row / 3) * 2;

                    int posX = margin + (int)(col * cellW) + thickLinesX;
                    int posY = margin + (int)(row * cellH) + thickLinesY;

                    int sizeX = margin + (int)((col + 1) * cellW) + thickLinesX - posX;
                    int sizeY = margin + (int)((row + 1) * cellH) + thickLinesY - posY;

                    txt.Location = new Point(posX, posY);
                    txt.Size = new Size(sizeX - 1, sizeY - 1);

                    bool isAlternateBlock = ((row / 3) + (col / 3)) % 2 != 0;
                    txt.BackColor = isAlternateBlock ? colorAltBack : colorDefaultBack;

                    txt.Tag = new int[] { row, col };

                    txt.KeyPress += Txt_KeyPress;
                    txt.TextChanged += Cell_TextChanged;

                    txt.Click += (s, ev) =>
                    {
                        ((TextBox)s).SelectAll();
                    };

                    txt.Enter += (s, ev) =>
                    {
                        TextBox currentTxt = (TextBox)s;
                        currentTxt.SelectAll();

                        int[] pos = (int[])currentTxt.Tag;
                        ApplyHighlight(pos[0], pos[1]);
                    };

                    sudokuCells[row, col] = txt;
                    pnlBoard.Controls.Add(txt);
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

                    if (r == row || c == col)
                        sudokuCells[r, c].BackColor = colorHighlightLine;
                    else
                        sudokuCells[r, c].BackColor = isAlt ? colorAltBack : colorDefaultBack;

                    if (!string.IsNullOrEmpty(targetValue) && sudokuCells[r, c].Text == targetValue)
                        sudokuCells[r, c].BackColor = colorSameNumber;
                }
            }
            sudokuCells[row, col].BackColor = colorFocus;
        }

        private void UpdateBoardColors()
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (!sudokuCells[r, c].ReadOnly && !string.IsNullOrEmpty(sudokuCells[r, c].Text))
                    {
                        if (int.TryParse(sudokuCells[r, c].Text, out int val))
                        {
                            if (val == logic.solutionBoard[r, c])
                            {
                                sudokuCells[r, c].ForeColor = Color.Blue;
                            }
                            else
                            {
                                sudokuCells[r, c].ForeColor = Color.Red;
                            }
                        }
                    }
                }
            }
        }

        private void Cell_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt == null || txt.Tag == null) return;
            int[] pos = (int[])txt.Tag;
            int r = pos[0], c = pos[1];

            if (int.TryParse(txt.Text, out int num)) logic.board[r, c] = num;
            else logic.board[r, c] = 0;

            UpdateBoardColors();
            ApplyHighlight(r, c);
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
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

        private void btnSurrender_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đầu hàng và thoát game không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}