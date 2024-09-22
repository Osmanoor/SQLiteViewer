using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Data.Sqlite;

namespace SQLiteViewer
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<AA_DistinctRoster> _aaDistinctRosterData;
        private ObservableCollection<BetterKillfeed> _betterKillfeedData;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            SetupFilterControls();
        }

        private void SetupFilterControls()
        {
            AA_DistinctRosterFilterControl.OriginalData = _aaDistinctRosterData;
            AA_DistinctRosterFilterControl.DataFiltered += AA_DistinctRosterFilterControl_DataFiltered;
            BetterKillfeedFilterControl.OriginalData = _betterKillfeedData;
            BetterKillfeedFilterControl.DataFiltered += BetterKillfeedFilterControl_DataFiltered;
        }

        private void LoadData()
        {
            _aaDistinctRosterData = new ObservableCollection<AA_DistinctRoster>();
            _betterKillfeedData = new ObservableCollection<BetterKillfeed>();

            string connectionString = "Data Source=Database.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                LoadAA_DistinctRosterData(connection);
                LoadBetterKillfeedData(connection);
            }

            ResultsDataGrid.ItemsSource = _aaDistinctRosterData;
        }

        private void LoadAA_DistinctRosterData(SqliteConnection connection)
        {
            string sql = "SELECT * FROM AA_DistinctRoster";
            using (var command = new SqliteCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _aaDistinctRosterData.Add(new AA_DistinctRoster
                        {
                            Num = reader.GetInt32(reader.GetOrdinal("Num")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Playlist = reader.GetString(reader.GetOrdinal("Playlist")),
                            PlayerId = reader.GetString(reader.GetOrdinal("PlayerId")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            Lvl = reader.GetInt32(reader.GetOrdinal("Lvl")),
                            Place = reader.GetInt32(reader.GetOrdinal("Place")),
                            Anon = reader.GetInt32(reader.GetOrdinal("Anon")),
                            Platform = reader.GetString(reader.GetOrdinal("Platform")),
                            Team = reader.GetInt32(reader.GetOrdinal("Team")),
                            Kills = reader.IsDBNull(reader.GetOrdinal("Kills")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Kills")),
                            BotKills = reader.IsDBNull(reader.GetOrdinal("BotKills")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("BotKills")),
                            Crowns = reader.GetInt32(reader.GetOrdinal("Crowns")),
                            TeamMate = reader.GetString(reader.GetOrdinal("TeamMate")),
                            Skin = reader.GetString(reader.GetOrdinal("Skin")),
                            Count = reader.GetInt32(reader.GetOrdinal("Count")),
                            MetK = reader.GetString(reader.GetOrdinal("MetK")),
                            MetD = reader.GetString(reader.GetOrdinal("MetD")),
                            Season = reader.GetString(reader.GetOrdinal("Season"))
                        });
                    }
                }
            }
        }

        private void LoadBetterKillfeedData(SqliteConnection connection)
        {
            string sql = "SELECT * FROM BetterKillfeed";
            using (var command = new SqliteCommand(sql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        _betterKillfeedData.Add(new BetterKillfeed
                        {
                            FileName = reader.GetString(reader.GetOrdinal("fileName")),
                            ActionNumber = reader.GetInt32(reader.GetOrdinal("actionNumber")),
                            Actioner = reader.GetString(reader.GetOrdinal("Actioner")),
                            Team1 = reader.GetInt32(reader.GetOrdinal("Team1")),
                            Actionee = reader.GetString(reader.GetOrdinal("Actionee")),
                            Team2 = reader.GetInt32(reader.GetOrdinal("Team2")),
                            Status = reader.GetString(reader.GetOrdinal("status")),
                            Action = reader.IsDBNull(reader.GetOrdinal("action")) ? null : reader.GetString(reader.GetOrdinal("action")),
                            Rarity = reader.GetString(reader.GetOrdinal("rarity")),
                            Weapon = reader.GetString(reader.GetOrdinal("weapon")),
                            POI = reader.IsDBNull(reader.GetOrdinal("POI")) ? null : reader.GetString(reader.GetOrdinal("POI")),
                            ActionTime = reader.GetFloat(reader.GetOrdinal("actionTime")),
                            ReplayDate = reader.GetDateTime(reader.GetOrdinal("replayDate")),
                            ActionerId = reader.GetString(reader.GetOrdinal("actionerId")),
                            ActioneeId = reader.GetString(reader.GetOrdinal("actioneeId"))
                        });
                    }
                }
            }
        }

        private void AA_DistinctRosterFilterControl_DataFiltered(object sender, ObservableCollection<AA_DistinctRoster> filteredData)
        {
            ResultsDataGrid.ItemsSource = filteredData;
        }

        private void BetterKillfeedFilterControl_DataFiltered(object sender, ObservableCollection<BetterKillfeed> filteredData)
        {
            ResultsDataGrid.ItemsSource = filteredData;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl)
            {
                switch (tabControl.SelectedIndex)
                {
                    case 0: // AA_DistinctRoster tab
                        ResultsDataGrid.ItemsSource = _aaDistinctRosterData;
                        break;
                    case 1: // BetterKillfeed tab
                        ResultsDataGrid.ItemsSource = _betterKillfeedData;
                        break;
                }
            }
        }
    }
}