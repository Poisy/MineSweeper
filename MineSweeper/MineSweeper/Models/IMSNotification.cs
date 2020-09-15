using Xamarin.Forms;

namespace MineSweeper.Models
{
    public interface IMSNotification
    {
        void NotifyGameOver(Cell cell);

        Grid RequestArea();
    }
}
