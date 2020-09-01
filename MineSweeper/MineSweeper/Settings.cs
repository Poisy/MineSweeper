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
        public string FlagSource { get; set; } = "flag_star";
        public string MineSource { get; set; } = "mine_flame";
        public string WrongMineSource { get; set; } = "mine_wrong";
    }
}
