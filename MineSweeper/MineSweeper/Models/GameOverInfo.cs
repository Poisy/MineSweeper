using System;

namespace MineSweeper.Models
{
    public class GameOverInfo
    {
        public bool DidWin { get; set; }
        public string Time { get; set; }
        public int MineLeft { get; set; }
        public int RightGuessedMines { get; set; }
        public int WrongGuessedMines { get; set; }
        public int AllMines { get; set; }
        public int FlagedMines { get; set; }
        public Action Restart { get; set; }
        public Action OpenRecords { get; set; }
    }
}
