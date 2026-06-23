using System;
using System.Windows.Forms;

namespace Sudoku.client
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // C?u h?nh kh?i t?o h? th?ng cho WinForms
            ApplicationConfiguration.Initialize();

            // CH? C?N S?A: Ch?y màn h?nh ðãng nh?p ð?u tiên thay v? Form tr?ng
            Application.Run(new LoginForm());
        }
    }
}