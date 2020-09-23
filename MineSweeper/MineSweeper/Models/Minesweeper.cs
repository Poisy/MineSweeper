using System;
using Xamarin.Forms;
using MineSweeper.Models.LongPress;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

namespace MineSweeper.Models
{
    /// <summary>
    /// Main object for the MineSweeper that contains mostly all functionality for the game
    /// </summary>
    public class Minesweeper : Grid, INotifyPropertyChanged
    {
        #region Properties and Fields
        private int minesLeft;

        /// <summary>
        /// Use this to change the settings of the game
        /// </summary>
        public IMSSettings Settings { get; set; }

        public bool IsGameOver { get; private set; }

        /// <summary>
        /// Bindable property that shows the how many mines are left (not flaged)
        /// </summary>
        public int MinesLeft
        {
            get => minesLeft;
            set
            {
                minesLeft = value;
                OnPropertyChanged(nameof(MinesLeft));
            }
        }

        /// <summary>
        /// This is used for preventing cells to be clicked
        /// </summary>
        private Point invalidCoordinates = new Point();

        private Point PreviousSize { get; set; }
        #endregion



        #region Constructor
        public Minesweeper(IMSSettings settings)
        {
            Settings = settings;
            MinesLeft = Settings.CountMines;

            LoadDefaultTheme();

            BindingContext = Settings;
        }
        #endregion



        #region Events

        /// <summary>
        /// The event is raisen when the game is over because of one of the followings:
        /// <br/>- wrong cell was clicked;
        /// <br/>- all mine cells are flagged and there are no flagged free cells.
        /// </summary>
        public event EventHandler<GameOverArgs> GameOver;

        /// <summary>
        /// The event is raised when any <see cref="Cell"/> is clicked
        /// </summary>
        public event EventHandler<CellClickedArgs> CellClicked;

        /// <summary>
        /// The event is raised when any <see cref="Cell"/> is pressed and holded
        /// </summary>
        public event EventHandler CellHolded;


        private void OnGameOver(GameOverArgs e) => GameOver?.Invoke(this, e);
        private void OnCellClicked(CellClickedArgs e) => CellClicked?.Invoke(this, e);
        private void OnCellHolded(EventArgs e) => CellHolded?.Invoke(this, e);

        #endregion



        #region Methods


        /// <summary>
        /// Defines a rows and columns in the <see cref="Grid"/> area
        /// </summary>
        public void CreateArea()
        {
            ColumnDefinitions.Clear();
            RowDefinitions.Clear();

            for (int i = 0; i < Settings.Rows; i++)
            {
                RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            }

            for (int i = 0; i < Settings.Columns; i++)
            {
                ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }
        }
        

        /// <summary>
        /// Creates <see cref="Cell"/> objects that are empty cells or mines. 
        /// The number of cells and mines depends in the <see cref="Settings"/> property
        /// </summary>
        public void CreateCells(bool needRestart = false)
        {
            MinesLeft = Settings.CountMines;

            if (needRestart)
            {
                for (int x = 0; x < Settings.Rows; x++)
                {
                    for (int y = 0; y < Settings.Columns; y++)
                    {
                        GetCell(x, y).Reset();
                    }
                }
            }
            else
            {
                for (int x = 0; x < Settings.Rows; x++)
                {
                    for (int y = 0; y < Settings.Columns; y++)
                    {
                        Children.Add(CreateCell(x, y));
                    }
                }
            }

            CreateMines();

            for (int x = 0; x < Settings.Rows; x++)
            {
                for (int y = 0; y < Settings.Columns; y++)
                {
                    GetCell(x, y).ScanNearbyCells(ViewsToCells(Children), Settings.Rows, Settings.Columns);
                }
            }

            PreviousSize = new Point(Settings.Rows, Settings.Columns);
        }


        /// <summary>
        /// Creates <see cref="Cell"/>'s of the Mine
        /// </summary>
        private void CreateMines()
        {
            Random rnd = new Random();

            for (int i = 0; i < MinesLeft; i++)
            {
                int x = rnd.Next(0, Settings.Rows);
                int y = rnd.Next(0, Settings.Columns);

                if (GetCell(x, y).IsMine)
                {
                    i--;

                    continue;
                }

                //Children[y * Settings.Rows + x] = new Cell(x, y, true);
                GetCell(x, y).IsMine = true;
            }
        }


        /// <summary>
        /// Creates cell objects (calls <see cref="CreateCells"/>) and refresh already defined visual Cells (<see cref="ImageButton"/>) 
        /// in the grid area
        /// </summary>
        public void Restart()
        {
            if (PreviousSize.X == Settings.Rows && PreviousSize.Y == Settings.Columns)
            {
                CreateCells(true);
            }
            else
            {
                CreateArea();

                Children.Clear();

                CreateCells();
            }

            IsGameOver = false;
        }


        /// <summary>
        /// Create single <see cref="Cell"/> in the given position.
        /// </summary>
        private Cell CreateCell(int x, int y, bool isMine = false)
        {
            Cell cell = new Cell(x, y, isMine);

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
            Cell cell = sender as Cell;

            int x = cell.Row; 
            int y = cell.Column;

            if (invalidCoordinates.X == x && invalidCoordinates.Y == y)
            {
                invalidCoordinates = new Point(-1, -1);
                return;
            }

            if (!cell.Show(ViewsToCells(Children), Settings.Rows, Settings.Columns))
            {
                int wrongMines = 0;

                for (int i = 0; i < Settings.Rows; i++)
                {
                    for (int j = 0; j < Settings.Columns; j++)
                    {
                        Cell temp = GetCell(i, j);

                        if (temp.IsMine && !temp.IsFlaged)
                        {
                            temp.SetSource(Settings.MineSource);
                            temp.Unhide();
                            continue;
                        }
                        if (!temp.IsMine && temp.IsFlaged)
                        {
                            temp.SetSource(Settings.WrongMineSource);
                            temp.Unhide();
                            wrongMines++;
                        }

                    }
                }

                IsGameOver = true;
                OnGameOver(new GameOverArgs { WrongMines = wrongMines, IsVictory = false });
                cell.BackgroundColor = Color.Red;
            }
            else
            {
                for (int i = 0; i < Settings.Rows; i++)
                {
                    for (int j = 0; j < Settings.Columns; j++)
                    {
                        Cell temp = GetCell(i, j);

                        if (temp.Visibility)
                        {
                            temp.Unhide();

                            if (temp.IsMine)
                            {
                                temp.SetSource(Settings.MineSource);
                            }
                            else
                            {
                                if (temp.MinesNerby > 0) temp.Source = "number_" + temp.MinesNerby;
                            }
                        }
                    }
                }
            }

            OnCellClicked(new CellClickedArgs { IsGameOver = IsGameOver });

            if (MinesLeft == 0)
            {
                if (DidWin())
                {
                    OnGameOver(new GameOverArgs { IsVictory = true, WrongMines = 0 });
                }
            }
        }


        /// <summary>
        /// Long press event for visual cell (<see cref="ImageButton"/>)
        /// </summary>
        private async void AddLongPressGesture (object sender, EventArgs e)
        {
            var temp = (sender as LongPressBehavior);

            int x = temp.X;
            int y = temp.Y;

            Cell cell = GetCell(x, y);

            if (cell.Visibility) return;

            if (cell.IsFlaged)
            {
                cell.Source = null;
                cell.IsFlaged = false;
                MinesLeft++;
            }
            else
            {
                cell.SetSource(Settings.FlagSource);
                cell.IsFlaged = true;
                MinesLeft--;

                if (MinesLeft == 0)
                {
                    if (DidWin())
                    {
                        OnGameOver(new GameOverArgs { IsVictory = true, WrongMines = 0 });
                    }
                }
            }

            OnCellHolded(new EventArgs());

            await Task.Run(() =>
            {
                Thread.Sleep(100);

                invalidCoordinates = new Point(x, y);
            });
        }


        /// <summary>
        /// Checks if all conditions for victory are achieved
        /// </summary>
        private bool DidWin()
        {
            foreach (Cell cell in ViewsToCells(Children))
            {
                if (cell.IsMine)
                {
                    if (!cell.IsFlaged) return false;
                }
                else
                {
                    if (!cell.Visibility) return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Sets the default settings
        /// </summary>
        private void LoadDefaultTheme()
        {
            RowSpacing = 0;
            ColumnSpacing = 0;
            //HorizontalOptions = LayoutOptions.Center;
            //VerticalOptions = LayoutOptions.Center;
            HeightRequest = 1000;
            WidthRequest = 500;
            BackgroundColor = Color.Azure;
        }


        /// <summary>
        /// Gets a <see cref="Cell"/> based of the given row and column
        /// </summary>
        private Cell GetCell(int row, int col)
        {
            Cell cell = Children[row * Settings.Columns + col] as Cell;

            //if (cell == null)
            //{
            //    throw new Exception("There can't be view in the Minesweeper which isn't Cell");
            //}

            return cell;
        }


        /// <summary>
        /// Convert <see cref="Grid.IGridList{T}"/> to a normal <see cref="List{T}"/>
        /// </summary>
        private List<Cell> ViewsToCells(IGridList<View> views)
        {
            return views.Cast<Cell>().ToList();
        }


        #endregion



        #region Classes

        public class GameOverArgs : EventArgs
        {
            public int WrongMines { get; set; }
            public bool IsVictory { get; set; }
        }

        public class CellClickedArgs : EventArgs
        {
            public bool IsGameOver { get; set; }
        }

        #endregion
    }
}