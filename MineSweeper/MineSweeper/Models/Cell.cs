using System.Collections.Generic;
using Xamarin.Forms;

namespace MineSweeper.Models
{
    /// <summary>
    /// Represents a single cell in MineSweeper
    /// </summary>
    public class Cell
    {
        #region Properties
        public int Row { get; private set; }

        public int Column { get; private set; }

        /// <summary>
        /// If the cell has been reviewed
        /// </summary>
        public bool Visibility { get; private set; } = false;

        /// <summary>
        /// If the cell is a empty or is a mine
        /// </summary>
        public bool IsMine { get; set; }

        /// <summary>
        /// How many mines are 1 block near the cell. <br/>
        /// Possible values from 0 to 8
        /// </summary>
        public int MinesNerby { get; private set; }

        /// <summary>
        /// If the cell has be flaged, that means you can't tap on it
        /// </summary>
        public bool IsFlaged { get; set; } = false;
        #endregion



        #region Constructors
        public Cell(int x, int y, bool isMine = false)
        {
            Column = x;
            Row = y;
            IsMine = isMine;
        }
        #endregion



        #region Methods
        /// <summary>
        /// If the cell is empty it makes itself visible and also the cells near it. Return true. <br/>
        /// If the cell is a mine, it shows only itself. Return false<br/>
        /// If the cell is a flaged, nothing happens. Return true
        /// </summary>
        /// <param name="cells">Used for showing off nearby cells</param>
        public bool Show(Cell[,] cells)
        {
            if (IsFlaged) return true;

            Visibility = true;

            if (IsMine) return false;

            ShowNearby(cells, Column, Row, new List<int[]>());

            return true;
        }


        /// <summary>
        /// Makes for each <see cref="Cell"/> <see cref="MinesNerby"/> equal to the number of the mines that are near 
        /// </summary>
        /// <param name="cells"></param>
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


        /// <summary>
        /// Recursive method for rapidly calling nearby cells to review themselfs
        /// </summary>
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
                    if (!cells[col, row].IsMine && !cells[col, row].IsFlaged)
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


        /// <summary>
        /// Used in <see cref="ShowNearby(Cell[,], int, int, List{int[]})"/>
        /// for checking if there are mines nearby the cell
        /// </summary>
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
        #endregion
    }
}
