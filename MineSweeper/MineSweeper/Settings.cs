using MineSweeper.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel;

namespace MineSweeper
{
    public class Settings : IMSSettings
    {
        private static Settings instance;
        private int areaSize;
        private int countMines;
        private Color background;
        private Color foreground;

        public event PropertyChangedEventHandler PropertyChanged;

        public int AreaSize
        {
            get => areaSize;
            set
            {
                if (value * (value / 2) >= countMines)
                {
                    areaSize = value;
                    OnPropertyChanged(nameof(AreaSize));
                }
            }
        }
        public int Rows { get { return AreaSize; } set { } }
        public int Columns { get { return AreaSize / 2; } set { } }
        public int CountMines
        {
            get => countMines;
            set
            {
                if (value <= areaSize * (areaSize / 2))
                {
                    countMines = value;
                    OnPropertyChanged(nameof(CountMines));
                }
            }
        }
        public Color Background
        {
            get => background;
            set
            {
                background = value;
                OnPropertyChanged(nameof(Background));
            }
        }
        public Color Foreground
        {
            get => foreground;
            set
            {
                foreground = value;
                OnPropertyChanged(nameof(Foreground));
            }
        }
        public string FlagSource { get; set; }
        public string MineSource { get; set; }
        public string WrongMineSource { get; set; }

        private void OnPropertyChanged(string name)
        {
            if (name == null || PropertyChanged == null) return;

            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private Dictionary<string, string> DataSettings { get; set; }

        private Settings() 
        {
            Load();
        }

        public static Settings GetSettings()
        {
            if (instance == null)
            {
                instance = new Settings();
            }

            return instance;
        }

        private void Load()
        {
            string path = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "settings.json");

            if (!File.Exists(path))
            {
                using (StreamWriter temp = new StreamWriter(path, true)) { temp.WriteLine("{}"); }
            }

            using (StreamReader temp = new StreamReader(path))
            {
                DataSettings = JsonConvert.DeserializeObject<Dictionary<string, string>>(temp.ReadToEnd());
            }

            try
            {
                AreaSize = int.Parse(DataSettings["area_size"]);
                CountMines = int.Parse(DataSettings["count_mines"]);
                Background = Xamarin.Forms.Color.FromHex(DataSettings["background"]);
                Foreground = Xamarin.Forms.Color.FromHex(DataSettings["foreground"]);
                FlagSource = DataSettings["flag_source"];
                MineSource = DataSettings["mine_source"];
                WrongMineSource = DataSettings["wrong_mine_source"];
            }
            catch (Exception) 
            {
                SetDefaultGameplaySettings();
                SetDefaultThemesSettings();
                FlagSource = "flag_star";
                MineSource = "mine_flame";
                WrongMineSource = "mine_wrong";
            }
   
        }

        public void Save()
        {
            DataSettings["area_size"] = AreaSize.ToString();
            DataSettings["count_mines"] = CountMines.ToString();
            DataSettings["background"] = ColorToHex(Background);
            DataSettings["foreground"] = ColorToHex(Foreground);
            DataSettings["flag_source"] = FlagSource;
            DataSettings["mine_source"] = MineSource;
            DataSettings["wrong_mine_source"] = WrongMineSource;

            File.WriteAllText(Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "settings.json"), 
                JsonConvert.SerializeObject(DataSettings));
        }

        public void SetDefaultThemesSettings()
        {
            Background = Color.LightGray;
            Foreground = Color.DimGray;
        }

        public void SetDefaultGameplaySettings()
        {
            AreaSize = 30;
            CountMines = 100;
        }

        private string ColorToHex(Color c)
        {
            return "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
        }
    }
}
