using Xamarin.Forms;

namespace MineSweeper.Models
{
    public interface IMSNotification
    {
        void NotifyGameOver(Cell cell);
        void NotifyMinesLeftChanged(int minesLeft);

        Grid RequestArea();
    }
}
