using System.Drawing;
using System.ComponentModel;

namespace MineSweeper.Models
{
    /// <summary>
    /// Interface used for implementing properties for MineSweeper
    /// </summary>
    public interface IMSSettings : INotifyPropertyChanged
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
