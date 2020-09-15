using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Amporis.Xamarin.Forms.ColorPicker;

namespace MineSweeper.Pages
{
    public class ThemeSettings : SettingsPageTemplate
    {
        public ThemeSettings() : base(5)
        {
            AddView(new Label { FontSize = 17, Text = "Background", TextColor = Color.White }, CreateBackgroundPicker());
            AddView(new Label { FontSize = 17, Text = "Foreground", TextColor = Color.White }, CreateForegroundPicker());

            AddDefaultButton(DefaultSettings);
        }

        StackLayout CreateBackgroundPicker()
        {
            StackLayout layout = new StackLayout();

            ColorPickerEntry colorPicker = new ColorPickerEntry
            { 
                Color = Settings.GetSettings().Background
            };

            colorPicker.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Color")
                {
                    Settings.GetSettings().Background = (sender as ColorPickerEntry).Color;
                    SettingsPage.DoMainPageNeedRestart = true;
                }
            };

            layout.Children.Add(colorPicker);

            return layout;
        }

        StackLayout CreateForegroundPicker()
        {
            StackLayout layout = new StackLayout();

            ColorPickerEntry colorPicker = new ColorPickerEntry
            {
                Color = Settings.GetSettings().Foreground
            };

            colorPicker.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "Color")
                {
                    Settings.GetSettings().Foreground = (sender as ColorPickerEntry).Color;
                    SettingsPage.DoMainPageNeedRestart = true;
                }
            };

            layout.Children.Add(colorPicker);

            return layout;
        }

        void DefaultSettings()
        {
            Settings.GetSettings().SetDefaultThemesSettings();

            List<View> views = GetOptions();

            (((views[0] as Frame).Content as StackLayout).Children[0] as ColorPickerEntry).Color = Settings.GetSettings().Background;
            (((views[1] as Frame).Content as StackLayout).Children[0] as ColorPickerEntry).Color = Settings.GetSettings().Foreground;
        }
    }
}
