using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MineSweeper.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPageTemplate : ContentPage
    {
        int maxElements;
        int count = 0;

        public SettingsPageTemplate(int maxElements)
        {
            InitializeComponent();

            this.maxElements = maxElements;

            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1.5, GridUnitType.Star) });

            while (maxElements != 0)
            {
                _grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
                maxElements--;
            }
        }

        public void AddView(Label text, View option)
        {
            if (count >= maxElements) return;

            FormatView(text);
            FormatView(option);

            Frame frame1 = CreateFrame(text);
            Frame frame2 = CreateFrame(option);

            Grid.SetColumn(frame1, 0);
            Grid.SetRow(frame1, count);

            Grid.SetColumn(frame2, 1);
            Grid.SetRow(frame2, count);

            _grid.Children.Add(frame1);
            _grid.Children.Add(frame2);

            count++;
        }

        public void AddDefaultButton(Action action)
        {
            _defaultButton.Clicked += (sender, e) => { action.Invoke(); };
        }

        public List<View> GetOptions()
        {
            List<View> views = new List<View>();
            int i = 1;

            while (i < count * 2)
            {
                views.Add(_grid.Children[i]);
                i += 2;
            }

            return views;
        }

        void FormatView(View view)
        {
            view.VerticalOptions = LayoutOptions.Center;
        }

        Frame CreateFrame(View view)
        {
            Frame frame = new Frame
            {
                Content = view,
                CornerRadius = 15,
                BackgroundColor = Color.FromHex("#aa333333")
            };

            return frame;
        }
    }
}