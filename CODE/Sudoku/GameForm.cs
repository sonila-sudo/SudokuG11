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

        private TextBox[,] sudokuCells = new TextBox[9, 9];

        private void GameForm_Load(object sender, EventArgs e)
        {
            CreateSudokuGrid();
        }

        private void CreateSudokuGrid()
        {
            int cellSize = 50;
            int borderThickness = 3; 

            pnlBoard.BackColor = Color.Navy;

            pnlBoard.Size = new Size(470, 470);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    TextBox txt = new TextBox();
                    txt.Size = new Size(cellSize, cellSize);

                    int thinLineX = col * 1;
                    int thinLineY = row * 1;

                    int thickLineX = (col / 3) * 3;
                    int thickLineY = (row / 3) * 3;

                    txt.Location = new Point(
                        (col * cellSize) + thinLineX + thickLineX + borderThickness,
                        (row * cellSize) + thinLineY + thickLineY + borderThickness
                    );

                    txt.Multiline = true;
                    txt.Font = new Font("Segoe UI", 18, FontStyle.Bold);
                    txt.ForeColor = Color.DarkSlateGray;
                    txt.TextAlign = HorizontalAlignment.Center;
                    txt.MaxLength = 1;
                    txt.BorderStyle = BorderStyle.None;

                    bool isAlternateBlock = ((row / 3) + (col / 3)) % 2 != 0;
                    txt.BackColor = isAlternateBlock ? Color.AliceBlue : Color.White;

                    sudokuCells[row, col] = txt;
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

        private void lblGameStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
