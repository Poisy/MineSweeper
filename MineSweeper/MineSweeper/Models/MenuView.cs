﻿using MineSweeper.Pages;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MineSweeper.Models
{
    public class MenuView : ListView
    {
        public static bool IsOpen { get; set; } = false;

        public MenuView(INavigation navigation, MainPage mainPage)
        {
            IsOpen = true;
            Opacity = 0;
            Scale = 0;

            Task.Run(async () => await BeginAnimation());

            Grid.SetColumn(this, 0);
            Grid.SetColumnSpan(this, 3);

            Grid.SetRow(this, 1);

            Margin = 50;

            BackgroundColor = Color.FromHex("#AAE64B86");
            HorizontalOptions = LayoutOptions.Center;

            ItemsSource = new MenuOption[] {
                new MenuOption { Icon = "\uf3e5", Name = "Resume" },
                new MenuOption { Icon = "\uf2f9", Name = "New Game" },
                new MenuOption { Icon = "\uf682", Name = "Records" },
                new MenuOption { Icon = "\uf013", Name = "Settings" }
            };

            ItemTemplate = new DataTemplate(typeof(CustomCell));
            ItemSelected += async (sender, e) => {
                var option = e.SelectedItem as MenuOption;

                if (option == null) return;

                switch (option.Name)
                {
                    case "Resume":
                        mainPage.CloseMenu();
                        break;
                    case "New Game":
                        mainPage.CloseMenu();
                        mainPage.Restart();
                        break;
                    case "Records":
                        await navigation.PushModalAsync(new RecordsPage(), true);
                        break;
                    case "Settings":
                        await navigation.PushModalAsync(new SettingsPage(mainPage), true);
                        break;
                }

                SelectedItem = null;
            };
        }

        public async Task BeginAnimation()
        {
            IsOpen = true;
            IsEnabled = true;
            await this.FadeTo(1);
            await this.ScaleTo(1);
        }

        public async Task CloseAnimation()
        {
            IsOpen = false;
            IsEnabled = false;
            await this.FadeTo(0);
            await this.ScaleTo(0);
        }

        class MenuOption
        {
            public string Icon { get; set; }
            public string Name { get; set; }
        }

        class CustomCell : ViewCell
        {
            public CustomCell()
            {
                Label icon = new Label
                {
                    Style = Application.Current.Resources["IconLabelStyle"] as Style
                };
                icon.SetBinding(Label.TextProperty, "Icon");
                Grid.SetColumn(icon, 0);

                Label name = new Label
                {
                    FontSize = 30,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.White,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start
                };
                name.SetBinding(Label.TextProperty, "Name");
                Grid.SetColumn(name, 1);

                Grid grid = new Grid();

                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

                grid.Children.Add(icon);
                grid.Children.Add(name);

                View = grid;
            }
        }
    }
}