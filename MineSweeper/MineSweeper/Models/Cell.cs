using System.Collections.Generic;
using Xamarin.Forms;

namespace MineSweeper.Models
{
    /// <summary>
    /// Represents a single cell in MineSweeper
    /// </summary>
    public class Cell : ImageButton
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
            Row = x;
            Column = y;
            IsMine = isMine;

            Grid.SetColumn(this, y);
            Grid.SetRow(this, x);

            SetBinding(ImageButton.BackgroundColorProperty, new Binding("Foreground"));
        }
        #endregion



        #region Methods


        /// <summary>
        /// Placing a image in the Cell
        /// </summary>
        public void SetSource(string source)
        {
            Source = source;
        }


        /// <summary>
        /// Shows the background of the Cell
        /// </summary>
        public void Unhide()
        {
            SetBinding(ImageButton.BackgroundColorProperty, new Binding("Background"));
        }


        /// <summary>
        /// Sets the Cell to the default settings
        /// </summary>
        public void Reset()
        {
            SetBinding(ImageButton.BackgroundColorProperty, new Binding("Foreground"));
            Source = null;
            IsMine = false;
            IsFlaged = false;
            Visibility = false;
        }


        /// <summary>
        /// If the cell is empty it makes itself visible and also the cells near it. Return true. <br/>
        /// If the cell is a mine, it shows only itself. Return false<br/>
        /// If the cell is a flaged, nothing happens. Return true
        /// </summary>
        /// <param name="cells">Used for showing off nearby cells</param>
        public bool Show(List<Cell> cells, int maxRow, int maxColumn)
        {
            if (IsFlaged) return true;

            Visibility = true;

            if (IsMine) return false;

            ShowNearby(cells, Row, Column, new List<int[]>(), maxRow, maxColumn);

            return true;
        }


        /// <summary>
        /// Makes for each <see cref="Cell"/> <see cref="MinesNerby"/> equal to the number of the mines that are near 
        /// </summary>
        /// <param name="cells"></param>
        public void ScanNearbyCells(List<Cell> cells, int maxRow, int maxColumn)
        {
            MinesNerby = 0;

            if (!IsMine)
            {
                for (int x = Row - 1; x <= Row + 1; x++)
                {
                    for (int y = Column - 1; y <= Column + 1; y++)
                    {
                        if (x == Row && y == Column) continue;

                        if (x >= 0 && x < maxRow)
                        {
                            if (y >= 0 && y < maxColumn)
                            {
                                if (cells[x * maxColumn + y].IsMine)
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
        private void ShowNearby(List<Cell> cells, int row, int col, List<int[]> visitedCells, int maxRow, int maxColumn)
        {
            foreach (var x in visitedCells)
            {
                if (x[0] == row && x[1] == col) return;
            }

            visitedCells.Add(new int[] { row, col });

            if (col >= 0 && col < maxColumn)
            {
                if (row >= 0 && row < maxRow)
                {
                    if (!cells[row * maxColumn + col].IsMine && !cells[row * maxColumn + col].IsFlaged)
                    {
                        cells[row * maxColumn + col].Visibility = true;

                        if (CheckNearbyForMines(cells, cells[row * maxColumn + col], maxRow, maxColumn))
                        {
                            ShowNearby(cells, row, col - 1, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row, col + 1, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row - 1, col, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row + 1, col, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row + 1, col - 1, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row - 1, col + 1, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row - 1, col - 1, visitedCells, maxRow, maxColumn);
                            ShowNearby(cells, row + 1, col + 1, visitedCells, maxRow, maxColumn);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Used in <see cref="ShowNearby(List{Cell}, int, int, List{int[]})"/>
        /// for checking if there are mines nearby the cell
        /// </summary>
        private bool CheckNearbyForMines(List<Cell> cells, Cell cell, int maxRow, int maxColumn)
        {
            if (cell.Row > 0)
            {
                if (cells[(cell.Row - 1) * maxColumn + cell.Column].IsMine) return false;
            }

            if (cell.Row < maxRow - 1)
            {
                if (cells[(cell.Row + 1) * maxColumn + cell.Column].IsMine) return false;
            }

            if (cell.Column > 0)
            {
                if (cells[cell.Row * maxColumn + cell.Column - 1].IsMine) return false;
            }

            if (cell.Column < maxColumn - 1)
            {
                if (cells[cell.Row * maxColumn + cell.Column + 1].IsMine) return false;
            }

            if (cell.Column > 0 && cell.Row > 0)
            {
                if (cells[(cell.Row - 1) * maxColumn + cell.Column - 1].IsMine) return false;
            }

            if (cell.Column < maxColumn - 1 && cell.Row < maxRow - 1)
            {
                if (cells[(cell.Row + 1) * maxColumn + cell.Column + 1].IsMine) return false;
            }

            if (cell.Column < maxColumn - 1 && cell.Row > 0)
            {
                if (cells[(cell.Row - 1) * maxColumn + cell.Column + 1].IsMine) return false;
            }

            if (cell.Column > 0 && cell.Row < maxRow - 1)
                {
                if (cells[(cell.Row + 1) * maxColumn + cell.Column - 1].IsMine) return false;
            }

            return true;
        }


        #endregion
    }
}
