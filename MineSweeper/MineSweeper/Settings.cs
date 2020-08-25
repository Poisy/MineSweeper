using MineSweeper.Models;
using System.Drawing;

namespace MineSweeper
{
    public class Settings : IMSSettings
    {
        private int number = 30;
        public int Rows { get { return number; } set { } }
        public int Columns { get { return number / 2; } set { } }
        public int CountMines { get; set; } = 100;
        public Color AreaBackground { get; set; } = Color.Azure;
        public Color CellBackground { get; set; } = Color.LightGray;
        public Color CellForeground { get; set; } = Color.DimGray;
        public Color CellTextColor { get; set; } = Color.Tomato;
        public Color CellMineColor { get; set; } = Color.Red;
    }
}
