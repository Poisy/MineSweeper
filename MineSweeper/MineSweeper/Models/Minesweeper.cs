using System;
using Xamarin.Forms;
using MineSweeper.Models.LongPress;

namespace MineSweeper.Models
{
    /// <summary>
    /// Main object for the MineSweeper that contains mostly all functionality for the game
    /// </summary>
    public class Minesweeper
    {
        #region Properties
        /// <summary>
        /// Contains all the cells
        /// </summary>
        public Cell[,] Cells { get; private set; }

        /// <summary>
        /// Use this to change the settings of the game
        /// </summary>
        public IMSSettings Settings { get; set; }

        public bool IsGameOver { get; private set; }

        public int MinesLeft { get; set; }

        /// <summary>
        /// Contains methods from the class who initialize this object
        /// </summary>
        private IMSNotification Notification { get; set; }
        #endregion



        #region Constructor
        public Minesweeper(IMSSettings settings, IMSNotification notification)
        {
            Settings = settings;
            Notification = notification;
            MinesLeft = Settings.CountMines;

            Notification.NotifyMinesLeftChanged(MinesLeft);
        }
        #endregion



        #region Methods
        /// <summary>
        /// Defines a rows and columns in the <see cref="Grid"/> area
        /// </summary>
        public void CreateArea(Grid _area)
        {
            _area.BackgroundColor = Settings.AreaBackground;

            for (int i = 0; i < Settings.Rows; i++)
            {
                _area.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            }

            for (int i = 0; i < Settings.Columns; i++)
            {
                _area.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }
        }
        

        /// <summary>
        /// Creates <see cref="Cell"/> objects that are empty cells or mines. 
        /// The number of cells and mines depends in the <see cref="Settings"/> property
        /// </summary>
        public void CreateCells()
        {
            Cells = new Cell[Settings.Columns, Settings.Rows];

            MinesLeft = Settings.CountMines;

            Notification.NotifyMinesLeftChanged(MinesLeft);

            for (int x = 0; x < Settings.Columns; x++)
            {
                for (int y = 0; y < Settings.Rows; y++)
                {
                    Cells[x, y] = new Cell(x, y, false);
                }
            }

            Random rnd = new Random();

            for (int i = 0; i < MinesLeft; i++)
            {
                int x = rnd.Next(0, Settings.Columns);
                int y = rnd.Next(0, Settings.Rows);

                if (Cells[x,y].IsMine)
                {
                    i--;

                    continue;
                }

                Cells[x, y] = new Cell(x, y, true);
            }
        }


        /// <summary>
        /// Creates visual objects (<see cref="ImageButton"/>) that represents Cells and adds in the grid area.
        /// The visual representation of the <see cref="Cell"/> depends of the <see cref="Settings"/> property
        /// </summary>
        public void DisplayCells(Grid _area)
        {
            for (int x = 0; x < Settings.Columns; x++)
            {
                for (int y = 0; y < Settings.Rows; y++)
                {
                    Cells[x, y].ScanNearbyCells(Cells);

                    _area.Children.Add(CreateCell(x, y));
                }
            }
        }


        /// <summary>
        /// Creates cell objects (calls <see cref="CreateCells"/>) and refresh already defined visual Cells (<see cref="ImageButton"/>) 
        /// in the grid area
        /// </summary>
        public void Restart(Grid _area)
        {
            CreateCells();

            for (int x = 0; x < Settings.Columns; x++)
            {
                for (int y = 0; y < Settings.Rows; y++)
                {
                    Cells[x, y].ScanNearbyCells(Cells);

                    ImageButton temp = (_area.Children[x * Settings.Rows + y] as ImageButton);

                    temp.BackgroundColor = Settings.CellForeground;
                    temp.Source = null;
                }
            }

            IsGameOver = false;
        }


        /// <summary>
        /// Create single <see cref="Cell"/> in the given position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private ImageButton CreateCell(int x, int y)
        {
            ImageButton cell = new ImageButton { BackgroundColor = Settings.CellForeground };

            Grid.SetRow(cell, y);
            Grid.SetColumn(cell, x);

            LongPressBehavior longPress = new LongPressBehavior(x, y, 500);

            longPress.LongPressed += AddLongPressGesture;

            cell.Behaviors.Add(longPress);
            cell.Clicked += AddTapGesture;

            return cell;
        }


        /// <summary>
        /// Tap event for visual cell (<see cref="ImageButton"/>)
        /// </summary>
        private void AddTapGesture(object sender, EventArgs e)
        {
            ImageButton cell = sender as ImageButton;

            int x = Grid.GetColumn(cell);
            int y = Grid.GetRow(cell);

            if (!Cells[x, y].Show(Cells))
            {
                for (int i = 0; i < Cells.GetLength(0); i++)
                {
                    for (int j = 0; j < Cells.GetLength(1); j++)
                    {
                        if (Cells[i, j].IsMine && !Cells[i, j].IsFlaged)
                        {
                            var temp = (cell.Parent as Grid).Children[i * Settings.Rows + j] as ImageButton;
                            temp.BackgroundColor = Settings.CellBackground;
                            temp.Source = Settings.MineSource;
                            continue;
                        }
                        if (!Cells[i, j].IsMine && Cells[i, j].IsFlaged)
                        {
                            var temp = (cell.Parent as Grid).Children[i * Settings.Rows + j] as ImageButton;
                            temp.BackgroundColor = Settings.CellBackground;
                            temp.Source = Settings.WrongMineSource;
                        }

                    }
                }

                cell.BackgroundColor = Color.Red;
                IsGameOver = true;
                Notification.NotifyGameOver(Cells[x, y]);
            }
            else
            {
                for (int i = 0; i < Cells.GetLength(0); i++)
                {
                    for (int j = 0; j < Cells.GetLength(1); j++)
                    {
                        if (Cells[i, j].Visibility)
                        {
                            var temp = (cell.Parent as Grid).Children[i * Settings.Rows + j] as ImageButton;
                            temp.BackgroundColor = Settings.CellBackground;

                            if (Cells[i, j].IsMine)
                            {
                                temp.Source = Settings.MineSource;
                            }
                            else
                            {
                                if (Cells[i, j].MinesNerby > 0) temp.Source = "number_" + Cells[i, j].MinesNerby;
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Long press event for visual cell (<see cref="ImageButton"/>)
        /// </summary>
        private void AddLongPressGesture (object sender, EventArgs e)
        {
            var temp = (sender as LongPressBehavior);

            int x = temp.X;
            int y = temp.Y;

            ImageButton cell = Notification.RequestArea().Children[x * Settings.Rows + y] as ImageButton;

            if (Cells[x, y].Visibility) return;

            if (Cells[x, y].IsFlaged)
            {
                cell.Source = null;
                Cells[x, y].IsFlaged = false;
                MinesLeft++;
            }
            else
            {
                cell.Source = Settings.FlagSource;
                Cells[x, y].IsFlaged = true;
                MinesLeft--;
            }

            Notification.NotifyMinesLeftChanged(MinesLeft);
        }
        #endregion
    }
}
