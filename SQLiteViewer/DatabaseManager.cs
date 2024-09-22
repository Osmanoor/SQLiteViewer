using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;

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
    }
}