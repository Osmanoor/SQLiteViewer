using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace SQLiteViewer
{
    public class BetterKillfeed : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public int ActionNumber { get; set; }
        public string Actioner { get; set; }
        public int Team1 { get; set; }
        public string Actionee { get; set; }
        public int Team2 { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
        public string Rarity { get; set; }
        public string Weapon { get; set; }
        public string POI { get; set; }
        public float ActionTime { get; set; }
        public DateTime ReplayDate { get; set; }
        public string ActionerId { get; set; }
        public string ActioneeId { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static ObservableCollection<BetterKillfeed> FilterByActionNumber(IEnumerable<BetterKillfeed> data, int minActionNumber, int maxActionNumber)
        {
            return new ObservableCollection<BetterKillfeed>(data.Where(item => item.ActionNumber >= minActionNumber && item.ActionNumber <= maxActionNumber));
        }

        public static ObservableCollection<BetterKillfeed> FilterByStatus(IEnumerable<BetterKillfeed> data, string status)
        {
            return new ObservableCollection<BetterKillfeed>(data.Where(item => item.Status == status));
        }

        public static ObservableCollection<BetterKillfeed> FilterByRarity(IEnumerable<BetterKillfeed> data, string rarity)
        {
            return new ObservableCollection<BetterKillfeed>(data.Where(item => item.Rarity == rarity));
        }

        public static ObservableCollection<BetterKillfeed> FilterByWeapon(IEnumerable<BetterKillfeed> data, string weapon)
        {
            return new ObservableCollection<BetterKillfeed>(data.Where(item => item.Weapon == weapon));
        }

        public static ObservableCollection<BetterKillfeed> FilterByPOI(IEnumerable<BetterKillfeed> data, string poi)
        {
            return new ObservableCollection<BetterKillfeed>(data.Where(item => item.POI == poi));
        }
    }
}