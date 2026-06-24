namespace Sudoku.Shared.Sudoku;

public class SudokuLogic
{
  private readonly Random _random = new();

  public int[,] Board { get; private set; } = new int[9, 9];
  public int[,] SolutionBoard { get; private set; } = new int[9, 9];

  public void GenerateRandomBoard(int emptyCells)
  {
    Board = new int[9, 9];
    SolutionBoard = new int[9, 9];
    FillDiagonalBlocks(SolutionBoard);
  SolveBoard(SolutionBoard);
    CopyBoard(SolutionBoard, Board);
    RemoveCells(Board, emptyCells);
  }

  public void LoadPuzzle(int[,] puzzle, int[,] solution)
  {
    Board = (int[,])puzzle.Clone();
    SolutionBoard = (int[,])solution.Clone();
  }

  public bool IsBoardComplete()
  {
    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
      if (Board[r, c] == 0 || Board[r, c] != SolutionBoard[r, c])
        return false;
    return true;
  }

  public bool IsCorrectMove(int row, int col, int value) =>
    value == SolutionBoard[row, col];

  public bool WasEmptyInPuzzle(int row, int col, int[,] initialPuzzle) =>
    initialPuzzle[row, col] == 0;

  public int CountCorrectPlayerCells(int[,] initialPuzzle)
  {
    var count = 0;
    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
    {
      if (initialPuzzle[r, c] != 0)
        continue;
      if (Board[r, c] != 0 && Board[r, c] == SolutionBoard[r, c])
        count++;
    }
    return count;
  }

  public int[,] BuildFullBoard()
  {
    var full = new int[9, 9];
    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
      full[r, c] = Board[r, c];
    return full;
  }

  private void FillDiagonalBlocks(int[,] grid)
  {
    for (var block = 0; block < 9; block += 3)
      FillBlock(grid, block, block);
  }

  private void FillBlock(int[,] grid, int startRow, int startCol)
  {
    var numbers = Enumerable.Range(1, 9).OrderBy(_ => _random.Next()).ToArray();
    var index = 0;
    for (var r = 0; r < 3; r++)
    for (var c = 0; c < 3; c++)
      grid[startRow + r, startCol + c] = numbers[index++];
  }

  private bool SolveBoard(int[,] grid)
  {
    for (var row = 0; row < 9; row++)
    {
      for (var col = 0; col < 9; col++)
      {
        if (grid[row, col] != 0)
          continue;

        var numbers = Enumerable.Range(1, 9).OrderBy(_ => _random.Next()).ToArray();
        foreach (var num in numbers)
        {
          if (!IsValid(grid, row, col, num))
            continue;

          grid[row, col] = num;
          if (SolveBoard(grid))
            return true;
          grid[row, col] = 0;
        }

        return false;
      }
    }

    return true;
  }

  private void RemoveCells(int[,] grid, int count)
  {
    var cells = Enumerable.Range(0, 81).OrderBy(_ => _random.Next()).Take(count);
    foreach (var cell in cells)
      grid[cell / 9, cell % 9] = 0;
  }

  private static bool IsValid(int[,] grid, int row, int col, int num)
  {
    for (var i = 0; i < 9; i++)
    {
      if (grid[row, i] == num || grid[i, col] == num)
        return false;
    }

    var startRow = row / 3 * 3;
    var startCol = col / 3 * 3;
    for (var r = 0; r < 3; r++)
    for (var c = 0; c < 3; c++)
      if (grid[startRow + r, startCol + c] == num)
        return false;

    return true;
  }

  private static void CopyBoard(int[,] source, int[,] target)
  {
    for (var r = 0; r < 9; r++)
    for (var c = 0; c < 9; c++)
      target[r, c] = source[r, c];
  }
}
