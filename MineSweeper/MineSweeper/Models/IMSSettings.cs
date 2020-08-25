using System.Drawing;

namespace MineSweeper.Models
{
    /// <summary>
    /// Interface used for implementing properties for MineSweeper
    /// </summary>
    public interface IMSSettings
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
