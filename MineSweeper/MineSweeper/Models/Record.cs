using System;
using SQLite;

namespace MineSweeper.Models
{
    public class Record
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Time { get; set; }
        public int MinesCount { get; set; }
        public string AreaSize { get; set; }
        public DateTime Date { get; set; }
        public string DateString => Date.ToString("MM/dd/yyyy HH:mm");

        public override string ToString()
        {
            return $"{Time} - {MinesCount} - {AreaSize} - {DateString}";
        }
    }
}
