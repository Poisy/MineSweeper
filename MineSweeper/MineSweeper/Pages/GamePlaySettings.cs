using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MineSweeper.Pages
{
    public class GamePlaySettings : SettingsPageTemplate
    {
        public GamePlaySettings() : base(5)
        {
            AddView(new Label { Text = "Total Mines", TextColor = Color.White, Padding = 5 }, CreateMinesCount());
            AddView(new Label { Text = "Total Cells", TextColor = Color.White, Padding = 5 }, CreateCellsCount());

            AddDefaultButton(DefaultSettings);
        }

        StackLayout CreateMinesCount()
        {
            Slider minesCountSlider = new Slider
            {
                Maximum = 300,
                Minimum = 1,
                Value = Settings.GetSettings().CountMines
            };

            minesCountSlider.ValueChanged += (sender, e) =>
            {
                double newValue = Math.Round(e.NewValue / 5) * 5;
                double oldValue = Math.Round(e.OldValue / 5) * 5;

                if (newValue > Settings.GetSettings().AreaSize * (Settings.GetSettings().AreaSize / 2))
                {
                    minesCountSlider.Value = oldValue;
                }
                else
                {
                    minesCountSlider.Value = newValue;
                    Settings.GetSettings().CountMines = (int)newValue;
                    SettingsPage.DoMainPageNeedRestart = true;
                }
            };

            Label minesCountsSliderText = new Label
            {
                BindingContext = minesCountSlider,
                TextColor = Color.White
            };

            minesCountsSliderText.SetBinding(Label.TextProperty, "Value");

            StackLayout layout = new StackLayout { Padding = 10 };

            layout.Children.Add(minesCountSlider);
            layout.Children.Add(minesCountsSliderText);

            return layout;
        }

        StackLayout CreateCellsCount()
        {
            Slider minesCountSlider = new Slider
            {
                Maximum = 50,
                Minimum = 1,
                Value = Settings.GetSettings().AreaSize
            };

            minesCountSlider.ValueChanged += (sender, e) =>
            {
                double newValue = Math.Round(e.NewValue);
                double oldValue = Math.Round(e.OldValue);

                if (newValue * (newValue / 2) < Settings.GetSettings().CountMines)
                {
                    minesCountSlider.Value = oldValue;
                }
                else
                {
                    minesCountSlider.Value = newValue;
                    Settings.GetSettings().AreaSize = (int)minesCountSlider.Value;
                    SettingsPage.DoMainPageNeedRestart = true;
                }
            };

            Label minesCountsSliderText = new Label
            {
                BindingContext = minesCountSlider,
                TextColor = Color.White
            };

            minesCountsSliderText.SetBinding(Label.TextProperty, "Value");

            StackLayout layout = new StackLayout { Padding = 10 };

            layout.Children.Add(minesCountSlider);
            layout.Children.Add(minesCountsSliderText);

            return layout;
        }

        void DefaultSettings()
        {
            Settings.GetSettings().SetDefaultGameplaySettings();

            List<View> views = GetOptions();

            (((views[0] as Frame).Content as StackLayout).Children[0] as Slider).Value = Settings.GetSettings().CountMines;
            (((views[1] as Frame).Content as StackLayout).Children[0] as Slider).Value = Settings.GetSettings().AreaSize;
        }
    }
}
