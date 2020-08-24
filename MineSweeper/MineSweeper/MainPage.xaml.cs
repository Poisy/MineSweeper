using MineSweeper.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;

namespace MineSweeper
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage, IMSNotification
    {
        private Minesweeper Minesweeper { get; set; } = new Minesweeper(new Settings());

        public MainPage()
        {
            InitializeComponent();

            Minesweeper.CreateArea(_area);

            Minesweeper.CreateCells();

            Minesweeper.DisplayCells(_area, this);
        }

        private void Restart()
        {
            Minesweeper.Restart(_area, this);
        }

        public void GameOver(Models.Cell cell)
        {
            Console.WriteLine("KABEEEEE!");
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Restart();
        }
    }
}
