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

            Minesweeper = new Minesweeper(new Settings(), this);

            Timer.Start(_timer);

            RecreateSettingsImage();

            Minesweeper.CreateArea(_area);

            Minesweeper.CreateCells();

            Minesweeper.DisplayCells(_area);
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            if (Minesweeper.IsGameOver) return;

            if (Timer.IsPaused)
            {
                Continue();
            }
            else
            {
                Pause();
            }
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

        private void Restart(object sender, EventArgs e)
        {
            Restart();
        }

        private void Restart()
        {
            Minesweeper.Restart(_area);

            Timer.Reset();
        }

        private void RecreateSettingsImage()
        {
            _settings.Source = "stamp_" + random.Next(1, 26).ToString() + ".png";
        }

        public void NotifyGameOver(Models.Cell cell)
        {
            Pause();
        }

        public void NotifyMinesLeftChanged(int minesLeft) => _mines.Text = minesLeft.ToString();

        public Grid RequestArea() => _area;
    }
}
