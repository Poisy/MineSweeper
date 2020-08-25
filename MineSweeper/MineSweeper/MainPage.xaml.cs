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

        public MainPage()
        {
            InitializeComponent();

            Minesweeper = new Minesweeper(new Settings(), this);

            Minesweeper.CreateArea(_area);

            Minesweeper.CreateCells();

            Minesweeper.DisplayCells(_area);
        }

        private void Restart()
        {
            Minesweeper.Restart(_area);
        }

        public void NotifyGameOver(Models.Cell cell)
        {
            Console.WriteLine("KABEEEEE!");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Restart();
        }

        public Grid RequestArea()
        {
            return _area;
        }
    }
}
