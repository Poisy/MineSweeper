using MineSweeper.Models;
using MineSweeper.Pages;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MineSweeper
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private Minesweeper Minesweeper { get; set; }
        private Random random = new Random();
        private MSTimer Timer { get; set; } = new MSTimer();
        private bool IsMenuAdded { get; set; } = false;
        public static bool HasGameStarted { get; set; } = false;
        private AudioPlayer AudioPlayer { get; set; } = new AudioPlayer();

        public MainPage()
        {
            InitializeComponent();

            Minesweeper = new Minesweeper(Settings.GetSettings());
            Minesweeper.GameOver += Stop;
            Minesweeper.CellClicked += Start;
            Minesweeper.CellHolded += CellHold;

            RecreateSettingsImage();

            Minesweeper.CreateArea();

            Minesweeper.CreateCells();

            BindingContext = Minesweeper;

            _mines.SetBinding(Label.TextProperty, new Binding("MinesLeft"));

            _area.Content = Minesweeper;


        }

        public void Start(object sender, Minesweeper.CellClickedArgs e)
        {
            AudioPlayer.Load(AudioPlayer.Sounds.CellClick);
            AudioPlayer.Play();

            if (e.IsGameOver) return;

            if (!Timer.IsPaused) _timer.Text = "00:00";

            if (!HasGameStarted)
            {
                if (MSTimer.HasStarted) Timer.Reset();
                else Timer.Start(_timer);
            }

            HasGameStarted = true;

            Continue();
        }

        public void Restart()
        {
            Minesweeper.Restart();

            RecreateSettingsImage();

            _timer.Text = "00:00";

            HasGameStarted = false;

            Pause();
        }

        public void Stop(object sender, Minesweeper.GameOverArgs e)
        {
            if (e.IsVictory)
            {
                Record record = new Record
                {
                    Time = Timer.Timer,
                    MinesCount = Settings.GetSettings().CountMines,
                    AreaSize = Settings.GetSettings().AreaSize.ToString() + "x" + (Settings.GetSettings().AreaSize/2).ToString(),
                    Date = DateTime.Now
                };

                RecordsDatabase.GetInstance().SaveRecord(record);

                AudioPlayer.Load(AudioPlayer.Sounds.Victory);
                AudioPlayer.Play();
            }
            else
            {
                if (Settings.GetSettings().CountMines / Minesweeper.MinesLeft > 10 && Settings.GetSettings().CountMines > 50)
                {
                    AudioPlayer.Load(AudioPlayer.Sounds.SpecialLose);
                }
                else
                {
                    AudioPlayer.Load(AudioPlayer.Sounds.Lose);
                }

                AudioPlayer.Play();
            }

            Pause();

            HasGameStarted = false;

            DisplayGameNotification(e.IsVictory, e.WrongMines);
        }

        public void Pause()
        {
            Timer.IsPaused = true;
            Minesweeper.IsEnabled = false;
        }

        private void Continue()
        {
            Timer.IsPaused = false;
            Minesweeper.IsEnabled = true;
        }

        private async void OpenMenu(object sender, EventArgs e)
        {
            if (NotificationPage.IsOpen) return;

            if (!IsMenuAdded)
            {
                _mainGrid.Children.Add(new MenuView(Navigation, this));
                Pause();
                IsMenuAdded = true;
                return;
            }

            if (!MenuView.IsOpen)
            {
                await (_mainGrid.Children[_mainGrid.Children.Count - 1] as MenuView).BeginAnimation();
                Pause();
            }
        }

        public async void CloseMenu()
        {
            await (_mainGrid.Children[_mainGrid.Children.Count - 1] as MenuView).CloseAnimation();
            if (HasGameStarted) Continue();
            else Minesweeper.IsEnabled = true;
        }

        private void RecreateSettingsImage()
        {
            _settings.Source = "stamp_" + random.Next(1, 26).ToString() + ".png";
        }

        private void DisplayGameNotification(bool isVictory, int wrongMines)
        {
            GameOverInfo info = new GameOverInfo
            {
                DidWin = isVictory,
                Time = Timer.Timer,
                MineLeft = Minesweeper.MinesLeft,
                RightGuessedMines = Settings.GetSettings().CountMines - Minesweeper.MinesLeft - wrongMines,
                WrongGuessedMines = wrongMines,
                AllMines = Settings.GetSettings().CountMines,
                FlagedMines = Settings.GetSettings().CountMines - Minesweeper.MinesLeft,
                Restart = () => { Restart(); Minesweeper.IsEnabled = true; },
                OpenRecords = NavigateThroughRecords
            };

            NotificationPage notification = new NotificationPage(info);

            _mainGrid.Children.Add(notification);

        }

        public async void NavigateThroughRecords()
        {
            await Navigation.PushModalAsync(new RecordsPage());
        }

        private void CellHold(object sender, EventArgs e)
        {
            AudioPlayer.Load(AudioPlayer.Sounds.CellHold);
            AudioPlayer.Play();
        }
    }
}
