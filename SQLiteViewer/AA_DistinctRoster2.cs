using System;
using System.ComponentModel;

namespace SQLiteViewer
{
    public class AA_DistinctRoster : INotifyPropertyChanged
    {
        public int Num { get; set; }
        public DateTime Date { get; set; }
        public string Playlist { get; set; }
        public string PlayerId { get; set; }
        public string DisplayName { get; set; }
        public int Lvl { get; set; }
        public int Place { get; set; }
        public int Anon { get; set; }
        public string Platform { get; set; }
        public int Team { get; set; }
        public int? Kills { get; set; }
        public int? BotKills { get; set; }
        public int Crowns { get; set; }
        public string TeamMate { get; set; }
        public string Skin { get; set; }
        public int Count { get; set; }
        public string MetK { get; set; }
        public string MetD { get; set; }
        public string Season { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
