namespace MineSweeper.Models
{
    public interface IMSNotification
    {
        void NotifyGameOver(Cell cell, int wrongMines);
    }
}
