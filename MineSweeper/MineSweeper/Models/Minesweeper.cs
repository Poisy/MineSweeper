using System;
using Xamarin.Forms;
using MineSweeper.Models.LongPress;
using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;

namespace MineSweeper.Models
{
    /// <summary>
    /// Main object for the MineSweeper that contains mostly all functionality for the game
    /// </summary>
    public class Minesweeper : INotifyPropertyChanged
    {
        #region Properties and Fields
        private int minesLeft;

        /// <summary>
        /// Contains all the cells
        /// </summary>
        public Cell[,] Cells { get; private set; }

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
        /// Contains methods from the class who initialize this object
        /// </summary>
        private IMSNotification Notification { get; set; }

        /// <summary>
        /// This is used for preventing cells to be clicked
        /// </summary>
        private Point invalidCoordinates = new Point();

        public event PropertyChangedEventHandler PropertyChanged;

        private Point PreviousSize { get; set; }
        #endregion



        #region Constructor
        public Minesweeper(IMSSettings settings, IMSNotification notification)
        {
            Settings = settings;
            Notification = notification;
            MinesLeft = Settings.CountMines;
        }
        #endregion



        #region Methods
        /// <summary>
        /// Defines a rows and columns in the <see cref="Grid"/> area
        /// </summary>
        public void CreateArea(Grid _area)
        {
            _area.BackgroundColor = Color.Azure;

            _area.ColumnDefinitions.Clear();
            _area.RowDefinitions.Clear();

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

            PreviousSize = new Point(Settings.Columns, Settings.Rows);
        }


        /// <summary>
        /// Creates cell objects (calls <see cref="CreateCells"/>) and refresh already defined visual Cells (<see cref="ImageButton"/>) 
        /// in the grid area
        /// </summary>
        public void Restart(Grid _area)
        {
            CreateArea(_area);

            CreateCells();

            if (PreviousSize.X != Settings.Columns || PreviousSize.Y != Settings.Rows)
            {
                _area.Children.Clear();

                DisplayCells(_area);

                PreviousSize = new Point(Settings.Columns, Settings.Rows);
            }
            else
            {
                for (int x = 0; x < Settings.Columns; x++)
                {
                    for (int y = 0; y < Settings.Rows; y++)
                    {
                        Cells[x, y].ScanNearbyCells(Cells);

                        ImageButton temp = (_area.Children[x * Settings.Rows + y] as ImageButton);

                        Grid.SetRow(temp, y);
                        Grid.SetColumn(temp, x);

                        temp.BackgroundColor = Settings.Foreground;
                        temp.Source = null;
                    }
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
            ImageButton cell = new ImageButton 
            { 
                BindingContext = Settings
            };

            cell.SetBinding(ImageButton.BackgroundColorProperty, new Binding("Foreground"));

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

            if (invalidCoordinates.X == x && invalidCoordinates.Y == y) return;

            if (!Cells[x, y].Show(Cells))
            {
                for (int i = 0; i < Cells.GetLength(0); i++)
                {
                    for (int j = 0; j < Cells.GetLength(1); j++)
                    {
                        if (Cells[i, j].IsMine && !Cells[i, j].IsFlaged)
                        {
                            var temp = (cell.Parent as Grid).Children[i * Settings.Rows + j] as ImageButton;
                            temp.SetBinding(ImageButton.BackgroundColorProperty, new Binding("Background"));
                            temp.Source = Settings.MineSource;
                            continue;
                        }
                        if (!Cells[i, j].IsMine && Cells[i, j].IsFlaged)
                        {
                            var temp = (cell.Parent as Grid).Children[i * Settings.Rows + j] as ImageButton;
                            temp.SetBinding(ImageButton.BackgroundColorProperty, new Binding("Background"));
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
                            temp.SetBinding(ImageButton.BackgroundColorProperty, new Binding("Background"));

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
        private async void AddLongPressGesture (object sender, EventArgs e)
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

            await Task.Run(() =>
            {
                Thread.Sleep(100);

                invalidCoordinates = new Point(x, y);
            });
        }

        /// <summary>
        /// Used for Binding
        /// </summary>
        private void OnPropertyChanged(string name)
        {
            if (name == null || PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
