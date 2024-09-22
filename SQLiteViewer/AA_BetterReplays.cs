using System;
using System.ComponentModel;

namespace SQLiteViewer  // Replace with your actual project namespace if different
{
    public class AA_BetterReplays : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string fileName;
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        private DateTime replayDate;
        public DateTime ReplayDate
        {
            get => replayDate;
            set
            {
                replayDate = value;
                OnPropertyChanged(nameof(ReplayDate));
            }
        }

        private string playlist;
        public string Playlist
        {
            get => playlist;
            set
            {
                playlist = value;
                OnPropertyChanged(nameof(Playlist));
            }
        }

        private string teammates;
        public string Teammates
        {
            get => teammates;
            set
            {
                teammates = value;
                OnPropertyChanged(nameof(Teammates));
            }
        }

        private double gameTime;
        public double GameTime
        {
            get => gameTime;
            set
            {
                gameTime = value;
                OnPropertyChanged(nameof(GameTime));
            }
        }

        private double season;
        public double Season
        {
            get => season;
            set
            {
                season = value;
                OnPropertyChanged(nameof(Season));
            }
        }

        private int botCount;
        public int BotCount
        {
            get => botCount;
            set
            {
                botCount = value;
                OnPropertyChanged(nameof(BotCount));
            }
        }

        private int kills;
        public int Kills
        {
            get => kills;
            set
            {
                kills = value;
                OnPropertyChanged(nameof(Kills));
            }
        }

        private int? botKills;
        public int? BotKills
        {
            get => botKills;
            set
            {
                botKills = value;
                OnPropertyChanged(nameof(BotKills));
            }
        }

        private int placement;
        public int Placement
        {
            get => placement;
            set
            {
                placement = value;
                OnPropertyChanged(nameof(Placement));
            }
        }

        private string ended;
        public string Ended
        {
            get => ended;
            set
            {
                ended = value;
                OnPropertyChanged(nameof(Ended));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}