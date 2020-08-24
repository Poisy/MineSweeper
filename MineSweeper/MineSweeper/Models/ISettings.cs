using System.Drawing;

namespace MineSweeper.Models
{
    public interface ISettings
    {
        int Rows { get; set; }
        int Columns { get; set; }
        int CountMines { get; set; }
        Color AreaBackground { get; set; }
        Color CellBackground { get; set; }
        Color CellForeground { get; set; }
        Color CellTextColor { get; set; }
        Color CellMineColor { get; set; }
    }
}
