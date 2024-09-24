using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace SQLiteViewer  // Replace with your actual project namespace if different
{
    public class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager(string dbPath)
        {
            connectionString = $"Data Source={dbPath}";
        }

        public ObservableCollection<AA_BetterReplays> GetAllReplays()
        {
            var replays = new ObservableCollection<AA_BetterReplays>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM AA_BetterReplays";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        replays.Add(new AA_BetterReplays
                        {
                            FileName = reader["FileName"].ToString(),
                            ReplayDate = DateTime.Parse(reader["ReplayDate"].ToString()),
                            Playlist = reader["Playlist"].ToString(),
                            Teammates = reader["Teammates"].ToString(),
                            GameTime = Convert.ToDouble(reader["GameTime"]),
                            Season = Convert.ToDouble(reader["Season"]),
                            BotCount = Convert.ToInt32(reader["BotCount"]),
                            Kills = Convert.ToInt32(reader["Kills"]),
                            BotKills = reader["BotKills"] != DBNull.Value ? Convert.ToInt32(reader["BotKills"]) : (int?)null,
                            Placement = Convert.ToInt32(reader["Placement"]),
                            Ended = reader["Ended"].ToString()
                        });
                    }
                }
            }
            return replays;
        }
        public ObservableCollection<AA_DistinctRoster> GetAllDistictRoaster()
        {
            var replays = new ObservableCollection<AA_DistinctRoster>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM AA_DistinctRoster";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        replays.Add(new AA_DistinctRoster
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
                            Skin = Path.Combine(Environment.CurrentDirectory, "Outfits", reader.GetString(reader.GetOrdinal("Skin")) + ".png"),
                            Count = reader.GetInt32(reader.GetOrdinal("Count")),
                            MetK = reader.GetString(reader.GetOrdinal("MetK")),
                            MetD = reader.GetString(reader.GetOrdinal("MetD")),
                            Season = reader.GetString(reader.GetOrdinal("Season"))
                        });
                    }
                }
            }
            return replays;
        }
        public ObservableCollection<BetterKillfeed> GetAllBetterKillfeed()
        {
            var replays = new ObservableCollection<BetterKillfeed>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM BetterKillfeed";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        replays.Add(new BetterKillfeed
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
            return replays;
        }
    }
}