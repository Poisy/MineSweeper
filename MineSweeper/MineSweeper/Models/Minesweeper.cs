using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MineSweeper.Models
{
    public class Minesweeper
    {
        #region Properties
        public Cell[,] Cells { get; private set; }
        public ISettings Settings { get; set; }
        public bool IsGameOver { get; private set; }
        #endregion
        #region Constructor
        public Minesweeper(ISettings settings)
        {
            Settings = settings;
        }
        #endregion
        #region Methods
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
        
        public void CreateCells()
        {
            Cells = new Cell[Settings.Columns, Settings.Rows];

            for (int x = 0; x < Settings.Columns; x++)
            {
                for (int y = 0; y < Settings.Rows; y++)
                {
                    Cells[x, y] = new Cell(x, y, false);
                }
            }

            Random rnd = new Random();

            for (int i = 0; i < Settings.CountMines; i++)
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

        public void DisplayCells(Grid _area, IMSNotification notification)
        {
            for (int x = 0; x < Settings.Columns; x++)
            {
                for (int y = 0; y < Settings.Rows; y++)
                {
                    Cells[x, y].ScanNearbyCells(Cells);

                    _area.Children.Add(CreateCell(x, y, notification, _area, Cells[x, y].IsMine));
                }
            }
        }

        public void Restart(Grid _area, IMSNotification notification)
        {
            CreateCells();

            for (int x = 0; x < Settings.Columns; x++)
            {
                for (int y = 0; y < Settings.Rows; y++)
                {
                    Cells[x, y].ScanNearbyCells(Cells);

                    ReCreateCell((_area.Children[x * Settings.Rows + y] as Grid), x, y, notification, _area);
                }
            }
        }

        private Grid CreateCell(int x, int y, IMSNotification notification, Grid area, bool isMine)
        {
            Grid cell = new Grid { BackgroundColor = Settings.CellBackground };

            if (isMine)
            {
                cell.Children.Add(new Image
                {
                    Source = "mine"
                });
            }
            else
            {
                cell.Children.Add(new Label()
                {
                    Text = Cells[x, y].MinesNerby == 0 ? "" : Cells[x, y].MinesNerby.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Settings.CellTextColor
                });
            }
            
            cell.Children.Add(new BoxView()
            {
                BackgroundColor = Settings.CellForeground,
                Margin = new Thickness(0.5),
            });

            Grid.SetRow(cell, y);
            Grid.SetColumn(cell, x);

            AddTapGesture(cell, notification, area);

            return cell;
        }

        private void AddTapGesture(Grid grid, IMSNotification notification, Grid area)
        {
            var tapGesture = new TapGestureRecognizer();

            tapGesture.Tapped += (s, e) => {
                var cell = s as Grid;

                cell.Children[cell.Children.Count-1].Opacity = 0;

                int x = Grid.GetColumn(cell);
                int y = Grid.GetRow(cell);

                if (!Cells[x, y].Show(Cells))
                {
                    notification.GameOver(Cells[x,y]);
                }
                else
                {
                    for (int i = 0; i < Cells.GetLength(0); i++)
                    {
                        for (int j = 0; j < Cells.GetLength(1); j++)
                        {
                            if (Cells[i, j].Visibility)
                            {
                                var temp = area.Children[i * Settings.Rows + j] as Grid;
                                temp.Children[temp.Children.Count - 1].Opacity = 0;
                            }
                        }
                    }
                }
            };

            grid.GestureRecognizers.Clear();

            grid.GestureRecognizers.Add(tapGesture);
        }

        private void ReCreateCell(Grid cell, int x, int y, IMSNotification notification, Grid area)
        {
            if (Cells[x, y].IsMine)
            {
                cell.Children[0] = new Image
                {
                    Source = "mine"
                };
            }
            else
            {
                cell.Children[0] = new Label
                {
                    Text = Cells[x, y].MinesNerby == 0 ? "" : Cells[x, y].MinesNerby.ToString(),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = Settings.CellTextColor
                };
            }

            cell.Children[1] = new BoxView
            {
                BackgroundColor = Settings.CellForeground,
                Margin = new Thickness(0.5),
            };

            Grid.SetRow(cell, y);
            Grid.SetColumn(cell, x);

            AddTapGesture(cell, notification, area);
        }
        #endregion
    }
}
