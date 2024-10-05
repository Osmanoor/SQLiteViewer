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
        public int GetRowCount(string tableName)
        {
            int rowCount = 0;

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                string query = $"SELECT COUNT(*) FROM {tableName}";

                using (var command = new SqliteCommand(query, connection))
                {
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }

            return rowCount;
        }
        public ObservableCollection<AA_BetterReplays> FilterAndPaginateBetterReplays(
            string teammate, string playlist,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<AA_BetterReplays>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "SELECT * FROM AA_BetterReplays WHERE 1 = 1";

                // Add filters dynamically
                if (!string.IsNullOrEmpty(teammate))
                {
                    command.CommandText += " AND teammates LIKE @teammate";
                    command.Parameters.AddWithValue("@teammate", $"%{teammate}%");
                }
                if (!string.IsNullOrEmpty(playlist))
                {
                    command.CommandText += " AND playlist LIKE @playlist";
                    command.Parameters.AddWithValue("@playlist", $"%{playlist}%");
                }

                // Sorting
                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                // Pagination
                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber) * pageSize);

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
        public ObservableCollection<AA_DistinctRoster> FilterAndPaginateDistinctRoster(
            string fileName, DateTime? replayDate, string displayName, int? minLvl, int? maxLvl,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<AA_DistinctRoster>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "SELECT * FROM AA_DistinctRoster WHERE 1 = 1";

                // Add filters dynamically
                if (!string.IsNullOrEmpty(fileName))
                {
                    command.CommandText += " AND Playlist LIKE @fileName";
                    command.Parameters.AddWithValue("@fileName", $"%{fileName}%");
                }

                if (replayDate.HasValue)
                {
                    command.CommandText += " AND Date = @replayDate";
                    command.Parameters.AddWithValue("@replayDate", replayDate.Value.ToString("yyyy-MM-dd"));
                }

                if (!string.IsNullOrEmpty(displayName))
                {
                    command.CommandText += " AND DisplayName LIKE @displayName";
                    command.Parameters.AddWithValue("@displayName", $"%{displayName}%");
                }

                if (minLvl.HasValue)
                {
                    command.CommandText += " AND Lvl >= @minLvl";
                    command.Parameters.AddWithValue("@minLvl", minLvl.Value);
                }

                if (maxLvl.HasValue)
                {
                    command.CommandText += " AND Lvl <= @maxLvl";
                    command.Parameters.AddWithValue("@maxLvl", maxLvl.Value);
                }

                // Sorting
                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                // Pagination
                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber) * pageSize);

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
        public ObservableCollection<A_RosterWithCount> FilterAndPaginateRosterWithCount(
            string fileName, DateTime? replayDate, string displayName, string isBot, string isTeam,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<A_RosterWithCount>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "SELECT * FROM A_RosterWithCount WHERE 1 = 1";

                // Add filters dynamically
                if (!string.IsNullOrEmpty(fileName))
                {
                    command.CommandText += " AND fileName LIKE @fileName";
                    command.Parameters.AddWithValue("@fileName", $"%{fileName}%");
                }

                if (replayDate.HasValue)
                {
                    var temp = replayDate.Value.ToString("MM/dd/yyyy h:mm:ss tt");
                    command.CommandText += $" AND Date LIKE \'{temp}\'";
                    //command.Parameters.AddWithValue("@replayDate",temp);
                }

                if (!string.IsNullOrEmpty(displayName))
                {
                    command.CommandText += " AND playerId LIKE @displayName";
                    command.Parameters.AddWithValue("@displayName", $"%{displayName}%");
                }
                if (!string.IsNullOrEmpty(isBot))
                {
                    command.CommandText += $" AND isBot = {isBot}";
                }
                if (!string.IsNullOrEmpty(isTeam))
                {
                    command.CommandText += $" AND isTeam = {isTeam}";
                }

                // Sorting
                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                // Pagination
                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber) * pageSize);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        replays.Add(new A_RosterWithCount
                        {
                            Num = reader.GetInt32(reader.GetOrdinal("Num")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Playlist = reader.GetString(reader.GetOrdinal("Playlist")),
                            PlayerId = reader.GetString(reader.GetOrdinal("PlayerId")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            Lvl = reader.GetInt32(reader.GetOrdinal("Lvl")),
                            Place = reader.GetInt32(reader.GetOrdinal("Place")),
                            Anon = reader.GetInt32(reader.GetOrdinal("Anon")),
                            Platform = reader.IsDBNull(reader.GetOrdinal("Platform")) ? "" : reader.GetString(reader.GetOrdinal("Platform")),
                            Team = reader.GetInt32(reader.GetOrdinal("Team")),
                            Kills = reader.IsDBNull(reader.GetOrdinal("Kills")) ? 0 : reader.GetInt32(reader.GetOrdinal("Kills")),
                            BotKills = reader.IsDBNull(reader.GetOrdinal("BotKills")) ? 0 : reader.GetInt32(reader.GetOrdinal("BotKills")),
                            Crowns = reader.GetInt32(reader.GetOrdinal("Crowns")),
                            TeamMate = reader.GetString(reader.GetOrdinal("TeamMate")),
                            Skin = Path.Combine(Environment.CurrentDirectory, "Outfits", reader.GetString(reader.GetOrdinal("Skin")) + ".png"),
                            Count = reader.GetInt32(reader.GetOrdinal("Count")),
                            MetK = reader.GetString(reader.GetOrdinal("MetK")),
                            MetD = reader.GetString(reader.GetOrdinal("MetD")),
                            Season = reader.GetString(reader.GetOrdinal("Season")),
                            FileName = reader.GetString(reader.GetOrdinal("FileName")),
                        });
                    }
                }
            }

            return replays;
        }
        public ObservableCollection<BetterKillfeed> FilterAndPaginateBetterKillFeed(
            string fileName, string actioner, string actionee,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<BetterKillfeed>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "SELECT * FROM BetterKillfeed WHERE 1 = 1";

                // Add filters dynamically
                if (!string.IsNullOrEmpty(fileName))
                {
                    command.CommandText += " AND fileName LIKE @fileName";
                    command.Parameters.AddWithValue("@fileName", $"%{fileName}%");
                }
                if (!string.IsNullOrEmpty(actioner))
                {
                    command.CommandText += " AND actionerId LIKE @actioner";
                    command.Parameters.AddWithValue("@actioner", $"%{actioner}%");
                }
                if (!string.IsNullOrEmpty(actionee))
                {
                    command.CommandText += " AND actioneeId LIKE @actionee";
                    command.Parameters.AddWithValue("@actionee", $"%{actionee}%");
                }
                // Sorting
                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                // Pagination
                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber) * pageSize);

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