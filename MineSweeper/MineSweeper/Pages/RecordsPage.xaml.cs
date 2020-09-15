using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        Grid CreateRecordCell(Record record)
        {
            Grid recordCell = new Grid();

            for (int i = 0; i < 4; i++)
            {
                recordCell.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }

            recordCell.Children.Add(CreateLabel(record.Time, 0));
            recordCell.Children.Add(CreateLabel(record.MinesCount, 1));
            recordCell.Children.Add(CreateLabel(record.AreaSize, 2));
            recordCell.Children.Add(CreateLabel(record.Date.ToString(), 3));

            return recordCell;
        }

        Label CreateLabel(string name, int column)
        {
            Label label = new Label 
            { 
                Text = name,
                // Customise Labels Here
            };

            Grid.SetColumn(label, column);

            return label;
        }
    }
}