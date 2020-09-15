using MineSweeper.Models;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MineSweeper
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, IMSNotification
    {
        private Minesweeper Minesweeper { get; set; }
        private Random random = new Random();
        private MSTimer Timer { get; set; } = new MSTimer();

        public MainPage()
        {
            InitializeComponent();

            Minesweeper = new Minesweeper(Settings.GetSettings(), this);

            Timer.Start(_timer);

            RecreateSettingsImage();

            Minesweeper.CreateArea(_area);

            Minesweeper.CreateCells();

            Minesweeper.DisplayCells(_area);
        }

        private void OpenMenu(object sender, EventArgs e)
        {
            if (_mainGrid.Children.Count == 5)
            {
                _mainGrid.Children.Add(new MenuView(Navigation, this));
                Pause();
                return;
            }

            if (_mainGrid.Children[_mainGrid.Children.Count - 1].IsVisible) return;

            _mainGrid.Children[_mainGrid.Children.Count - 1].IsVisible = true;
            Pause();
        }

        public void CloseMenu()
        {
            _mainGrid.Children[_mainGrid.Children.Count - 1].IsVisible = false;
            Continue();
        }

        private void Pause()
        {
            Timer.IsPaused = true;

            _area.IsEnabled = false;
        }

        private void Continue()
        {
            Timer.IsPaused = false;

            _area.IsEnabled = true;
        }

        public void Restart()
        {
            Minesweeper.Restart(_area);

            Timer.Reset();
        }

        private void RecreateSettingsImage()
        {
            _settings.Source = "stamp_" + random.Next(1, 26).ToString() + ".png";
        }

        private async void DisplayGameNotification()
        {
            string title;
            string text;

            if (Minesweeper.MinesLeft > 0)
            {
                title = "Failed";
                text = "BOOM!! You didn't survive that!";
            }
            else
            {
                title = "Victory!";
                text = "Good job!";
            }

            await DisplayAlert(title, text, "OK");
        }

        public void NotifyGameOver(Models.Cell cell)
        {
            Pause();

            DisplayGameNotification();
        }

        public void NotifyMinesLeftChanged(int minesLeft) => _mines.Text = minesLeft.ToString();

        public Grid RequestArea() => _area;
    }
}
