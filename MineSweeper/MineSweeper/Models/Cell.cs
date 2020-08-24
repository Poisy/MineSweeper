using System.Collections.Generic;

namespace MineSweeper.Models
{
    public class Cell
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public bool Visibility { get; private set; } = false;
        public bool IsMine { get; set; }
        public int MinesNerby { get; private set; }

        public Cell(int x, int y, bool isMine = false)
        {
            Column = x;
            Row = y;
            IsMine = isMine;
        }

        public bool Show(Cell[,] cells)
        {
            Visibility = true;

            if (IsMine) return false;

            ShowNearby(cells, Column, Row, new List<int[]>());

            return true;
        }

        private void ShowNearby(Cell[,] cells, int col, int row, List<int[]> visitedCells)
        {
            foreach (var x in visitedCells)
            {
                if (x[0] == col && x[1] == row) return;
            }

            visitedCells.Add(new int[] { col, row });

            if (col >= 0 && col < cells.GetLength(0))
            {
                if (row >= 0 && row < cells.GetLength(1))
                {
                    if (!cells[col, row].IsMine)
                    {
                        cells[col, row].Visibility = true;

                        if (CheckNearbyForMines(cells, cells[col, row]))
                        {
                            ShowNearby(cells, col - 1, row, visitedCells);
                            ShowNearby(cells, col + 1, row, visitedCells);
                            ShowNearby(cells, col, row - 1, visitedCells);
                            ShowNearby(cells, col, row + 1, visitedCells);
                            ShowNearby(cells, col - 1, row + 1, visitedCells);
                            ShowNearby(cells, col + 1, row - 1, visitedCells);
                            ShowNearby(cells, col - 1, row - 1, visitedCells);
                            ShowNearby(cells, col + 1, row + 1, visitedCells);
                        }
                    }
                }
            }
        }

        private bool CheckNearbyForMines(Cell[,] cells, Cell cell)
        {
            int colLength = cells.GetLength(0);
            int rowLength = cells.GetLength(1);

            if (cell.Column > 0)
            {
                if (cells[cell.Column - 1, cell.Row].IsMine) return false;
            }

            if (cell.Column < colLength - 1)
            {
                if (cells[cell.Column + 1, cell.Row].IsMine) return false;
            }

            if (cell.Row > 0)
            {
                if (cells[cell.Column, cell.Row - 1].IsMine) return false;
            }

            if (cell.Row < rowLength - 1)
            {
                if (cells[cell.Column, cell.Row + 1].IsMine) return false;
            }

            if (cell.Column > 0 && cell.Row > 0)
            {
                if (cells[cell.Column - 1, cell.Row - 1].IsMine) return false;
            }

            if (cell.Column < colLength - 1 && cell.Row < rowLength - 1)
            {
                if (cells[cell.Column + 1, cell.Row + 1].IsMine) return false;
            }
            
            if (cell.Column > 0 && cell.Row < rowLength - 1)
            {
                if (cells[cell.Column - 1, cell.Row + 1].IsMine) return false;
            }

            if (cell.Column < colLength - 1 && cell.Row > 0)
            {
                if (cells[cell.Column + 1, cell.Row - 1].IsMine) return false;
            }

            return true;
        }

        public void ScanNearbyCells(Cell[,] cells)
        {
            MinesNerby = 0;

            if (!IsMine)
            {
                for (int x = Column - 1; x <= Column + 1; x++)
                {
                    for (int y = Row - 1; y <= Row + 1; y++)
                    {
                        if (x == Column && y == Row) continue;

                        if (x >= 0 && x < cells.GetLength(0))
                        {
                            if (y >= 0 && y < cells.GetLength(1))
                            {
                                if (cells[x, y].IsMine)
                                {
                                    MinesNerby++;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
