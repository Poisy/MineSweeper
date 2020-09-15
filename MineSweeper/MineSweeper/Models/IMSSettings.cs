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
        Color Background { get; set; }
        Color Foreground { get; set; }
        string FlagSource { get; set; }
        string MineSource { get; set; }
        string WrongMineSource { get; set; }
    }
}
