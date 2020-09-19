using MineSweeper.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MineSweeper.Pages
{
    public partial class NotificationPage : Frame
    {
        GameOverInfo Info { get; set; }
        Frame Frame { get; set; } = new Frame();
        public static bool IsOpen { get; set; } = false;

        public NotificationPage(GameOverInfo info)
        {
            IsOpen = true;
            ScaleX = 0;

            Info = info;

            Style = Application.Current.Resources["GameOverOutsideFrameStyle"] as Style;
            Frame.Style = Application.Current.Resources["GameOverInsideFrameStyle"] as Style;

            Task.Run(async () => await BeginAnimation(this));

            Initialize();
        }

        public async Task BeginAnimation(View view, uint duration = 250)
        {
            await view.ScaleXTo(1, duration);
        }

        public async Task CloseAnimation(View view, uint duration = 250)
        {
            await view.FadeTo(0, duration);
        }

        void Initialize()
        {
            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(2, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Label timeLabel = new Label { Text = "Time - " + Info.Time, TextColor = Color.White };
            Label messageLabel = new Label { Text = Info.DidWin ? "You Win!" : "You Lose!", TextColor = Color.White };

            if (Info.DidWin)
            {
                ImageButton recordsButton = new ImageButton 
                {
                    Style = Application.Current.Resources["NewGameImageButtonStyle"] as Style,
                    Source = "records_button"
                };
                recordsButton.Clicked += (sender, e) => { Info.OpenRecords.Invoke(); };

                Grid.SetRow(recordsButton, 2);
                grid.Children.Add(recordsButton);

                Task.Run(async () => await BeginAnimation(recordsButton, 500));
            }
            else
            {
                Label mineLeftLabel = new Label 
                { 
                    Text = $"You guessed {Info.RightGuessedMines} from {Info.AllMines} and wrong flaged {Info.WrongGuessedMines}", 
                    TextColor = Color.White
                };

                Grid.SetRow(mineLeftLabel, 2);
                grid.Children.Add(mineLeftLabel);
            }

            ImageButton newGameButton = new ImageButton 
            { 
                Style = Application.Current.Resources["NewGameImageButtonStyle"] as Style,
                Source = "newgame_button"
            };
            newGameButton.Clicked += async (sender, e) =>
            {
                await CloseAnimation(this);

                Info.Restart.Invoke();
                
                Close();
            };

            Task.Run(async () => await BeginAnimation(newGameButton, 500));

            ImageButton hideButton = new ImageButton
            {
                Style = Application.Current.Resources["NewGameImageButtonStyle"] as Style,
                Source = "hide_option",
                ScaleX = 1,
                VerticalOptions = LayoutOptions.End
            };
            hideButton.Pressed += async (sender, e) => { await this.FadeTo(0); };
            hideButton.Released += async (sender, e) => { await this.FadeTo(1); };

            Grid.SetRow(timeLabel, 0);
            Grid.SetRow(messageLabel, 1);
            Grid.SetRow(newGameButton, 3);
            Grid.SetRow(hideButton, 4);

            grid.Children.Add(timeLabel);
            grid.Children.Add(messageLabel);
            grid.Children.Add(newGameButton);
            grid.Children.Add(hideButton);

            Frame.Content = grid;

            Content = Frame;
        }

        void Close()
        {
            IsOpen = false;

            Grid parent = Parent as Grid;

            if ((parent.Children[parent.Children.Count - 1] as NotificationPage) != null)
            {
                parent.Children.RemoveAt(parent.Children.Count - 1);
            }
        }
    }
}
