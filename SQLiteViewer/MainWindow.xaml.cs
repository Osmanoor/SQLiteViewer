using System;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Data.Sqlite;

namespace SQLiteViewer
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<AA_DistinctRoster> _originalData;

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
            FilterControl.OriginalData = _originalData;
            FilterControl.DataFiltered += FilterControl_DataFiltered;
        }

        private void LoadData()
        {
            _originalData = new ObservableCollection<AA_DistinctRoster>();

            string connectionString = "Data Source=Da" +
                "tabase.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM AA_DistinctRoster";
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            _originalData.Add(new AA_DistinctRoster
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

            ResultsDataGrid.ItemsSource = _originalData;
        }

        private void FilterControl_DataFiltered(object sender, ObservableCollection<AA_DistinctRoster> filteredData)
        {
            ResultsDataGrid.ItemsSource = filteredData;
        }
    }
}