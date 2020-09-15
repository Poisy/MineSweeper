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
                BindingContext = Settings.GetSettings()
            };

            minesCountSlider.SetBinding(Slider.ValueProperty, new Binding("CountMines"));

            minesCountSlider.ValueChanged += (sender, e) =>
            {
                if (e.NewValue > Settings.GetSettings().AreaSize * (Settings.GetSettings().AreaSize / 2))
                {
                    minesCountSlider.Value = Math.Round(e.OldValue);
                }
                else
                {
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
                BindingContext = Settings.GetSettings()
            };

            minesCountSlider.SetBinding(Slider.ValueProperty, new Binding("AreaSize"));

            minesCountSlider.ValueChanged += (sender, e) =>
            {
                if (e.NewValue * (e.NewValue / 2) < Settings.GetSettings().CountMines)
                {
                    minesCountSlider.Value = Math.Round(e.OldValue);
                }
                else
                {
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
