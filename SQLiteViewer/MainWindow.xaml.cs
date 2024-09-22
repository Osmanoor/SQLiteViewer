using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Linq;
using SQLitePCL;

namespace SQLiteViewer
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<AA_BetterReplays> Replays { get; set; }
        private ObservableCollection<AA_BetterReplays> originalReplays;
        private readonly DatabaseManager dbManager;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize SQLitePCL.Batteries
            Batteries_V2.Init();

            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database.db");
            dbManager = new DatabaseManager(dbPath);
            LoadDataFromDatabase();
            DataContext = this;
        }

        private void LoadDataFromDatabase()
        {
            originalReplays = dbManager.GetAllReplays();
            Replays = new ObservableCollection<AA_BetterReplays>(originalReplays);
            ReplaysDataGrid.ItemsSource = Replays;
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            StartDatePicker.SelectedDate = null;
            EndDatePicker.SelectedDate = null;
            PlaylistComboBox.SelectedIndex = 0;
            MinKillsTextBox.Text = "";
            MaxKillsTextBox.Text = "";
            SeasonTextBox.Text = "";
            PlacementTextBox.Text = "";

            Replays = new ObservableCollection<AA_BetterReplays>(originalReplays);
            ReplaysDataGrid.ItemsSource = Replays;
        }

        private void ApplyFilters()
        {
            var filteredReplays = originalReplays.AsEnumerable();

            // Date Range Filter
            if (StartDatePicker.SelectedDate.HasValue && EndDatePicker.SelectedDate.HasValue)
            {
                filteredReplays = filteredReplays.Where(r => r.ReplayDate >= StartDatePicker.SelectedDate.Value && r.ReplayDate <= EndDatePicker.SelectedDate.Value);
            }

            // Playlist Filter
            if (PlaylistComboBox.SelectedItem is ComboBoxItem selectedPlaylist && selectedPlaylist.Content.ToString() != "All")
            {
                filteredReplays = filteredReplays.Where(r => r.Playlist == selectedPlaylist.Content.ToString());
            }

            // Kills Range Filter
            if (int.TryParse(MinKillsTextBox.Text, out int minKills) && int.TryParse(MaxKillsTextBox.Text, out int maxKills))
            {
                filteredReplays = filteredReplays.Where(r => r.Kills >= minKills && r.Kills <= maxKills);
            }

            // Season Filter
            if (double.TryParse(SeasonTextBox.Text, out double season))
            {
                filteredReplays = filteredReplays.Where(r => r.Season == season);
            }

            // Placement Filter
            if (int.TryParse(PlacementTextBox.Text, out int placement))
            {
                filteredReplays = filteredReplays.Where(r => r.Placement == placement);
            }

            Replays = new ObservableCollection<AA_BetterReplays>(filteredReplays);
            ReplaysDataGrid.ItemsSource = Replays;
        }

        private void DateFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PlaylistFilter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void KillsFilter_Changed(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SeasonFilter_Changed(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void PlacementFilter_Changed(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }
    }
}