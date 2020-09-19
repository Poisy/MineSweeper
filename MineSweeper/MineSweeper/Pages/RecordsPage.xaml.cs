using MineSweeper.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MineSweeper.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecordsPage : ContentPage
    {
        public RecordsPage()
        {
            InitializeComponent();

            _recordsListView.ItemsSource = RecordsDatabase.GetInstance().GetRecords().Result;

            _recordsListView.ItemTemplate = new DataTemplate(typeof(RecordCell));

            _recordsListView.ItemTapped += RecordSelected;

            _recordsListView.RowHeight = 50;
        }

        private async void RecordSelected(object sender, ItemTappedEventArgs e)
        {
            Record record = e.Item as Record;

            bool willDeleteRecord = await DisplayAlert(record.ToString(), "Do you want to delete this record ?", "Yes", "No");

            if (willDeleteRecord)
            {
                await RecordsDatabase.GetInstance().DeleteRecord(record);

                _recordsListView.ItemsSource = RecordsDatabase.GetInstance().GetRecords().Result;
            }
        }

        class RecordCell : ViewCell
        {
            public RecordCell()
            {
                Grid recordCell = new Grid
                {
                    Padding = new Thickness(3, 0, 3, 0),
                    Margin = 5,
                    BackgroundColor = Color.FromHex("#AAE64B86")
                };

                recordCell.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                recordCell.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                recordCell.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                recordCell.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

                recordCell.Children.Add(CreateLabel("Time", 0, 20));
                recordCell.Children.Add(CreateLabel("MinesCount", 1, 20));
                recordCell.Children.Add(CreateLabel("AreaSize", 2, 20));
                recordCell.Children.Add(CreateLabel("DateString", 3, 15));

                View = recordCell;
            }

            Label CreateLabel(string propName, int column, int size)
            {
                Label label = new Label
                {
                    FontSize = size,
                    TextColor = Color.White,
                    VerticalTextAlignment = TextAlignment.Center
                };

                label.SetBinding(Label.TextProperty, propName);
                Grid.SetColumn(label, column);

                return label;
            }
        }
    }
}