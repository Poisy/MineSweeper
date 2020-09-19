using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SQLite;

namespace MineSweeper.Models
{
    public class RecordsDatabase
    {
        private readonly SQLiteAsyncConnection database;
        private static RecordsDatabase Instance { get; set; }
        private static readonly string connectionString = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "records.db3");

        private RecordsDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<Record>().Wait();
        }

        public static RecordsDatabase GetInstance()
        {
            if (Instance == null)
            {
                Instance = new RecordsDatabase(connectionString);
            }
            return Instance;
        }

        public Task<List<Record>> GetRecords()
        {
            return database.Table<Record>().OrderBy(rec => rec.Date).ToListAsync();
        }

        public Task<int> SaveRecord(Record record)
        {
            if (record.ID != 0)
            {
                return database.UpdateAsync(record);
            }
            else
            {
                return database.InsertAsync(record);
            }
        }

        public Task<int> DeleteRecord(Record record)
        {
            return database.DeleteAsync(record);
        }
    }
}
