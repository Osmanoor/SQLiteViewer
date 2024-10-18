using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

namespace SQLiteViewer
{
    public class A_RosterWithCount : INotifyPropertyChanged
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
    public int Kills { get; set; }
    public int BotKills { get; set; }
    public int Crowns { get; set; }
    public string TeamMate { get; set; }
    public string Skin { get; set; }
    public int Count { get; set; }
    public string MetK { get; set; }
    public string MetD { get; set; }
    public string Season { get; set; }
    public string FileName { get; set; }
    public BitmapSource BitmapSource { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static IEnumerable<AA_DistinctRoster> FilterByLvlRange(IEnumerable<AA_DistinctRoster> data, int minLvl, int maxLvl)
    {
        return new ObservableCollection<AA_DistinctRoster>(data.Where(item => item.Lvl >= minLvl && item.Lvl <= maxLvl));
    }

    public static ObservableCollection<AA_DistinctRoster> FilterByPlaceRange(IEnumerable<AA_DistinctRoster> data, int minPlace, int maxPlace)
    {
        return new ObservableCollection<AA_DistinctRoster>(data.Where(item => item.Place >= minPlace && item.Place <= maxPlace));
    }

    public static ObservableCollection<AA_DistinctRoster> FilterByPlatform(IEnumerable<AA_DistinctRoster> data, string platform)
    {
        return new ObservableCollection<AA_DistinctRoster>(data.Where(item => item.Platform == platform));
    }

    public static ObservableCollection<AA_DistinctRoster> FilterByKills(IEnumerable<AA_DistinctRoster> data, int minKills)
    {
        return new ObservableCollection<AA_DistinctRoster>(data.Where(item => item.Kills.HasValue && item.Kills.Value >= minKills));
    }

    public static ObservableCollection<AA_DistinctRoster> FilterByTeam(IEnumerable<AA_DistinctRoster> data, int team)
    {
        return new ObservableCollection<AA_DistinctRoster>(data.Where(item => item.Team == team));
    }
}
}