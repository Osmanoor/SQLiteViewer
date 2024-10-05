using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;

namespace SQLiteViewer
{
    public class DatabaseManager2
    {
        private readonly string connectionString;

        public DatabaseManager2(string dbPath)
        {
            connectionString = $"Data Source={dbPath}";
        }

        // Method for AA_DistinctRoster with dynamic filters, pagination, and sorting
        public ObservableCollection<AA_DistinctRoster> FilterAndPaginateDistinctRoster(
            string fileName, DateTime? replayDate, string displayName, int? minLvl, int? maxLvl,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<AA_DistinctRoster>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM AA_DistinctRoster WHERE 1 = 1";

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

                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);

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
                            Skin = reader.GetString(reader.GetOrdinal("Skin")),
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

        // Method for AA_BetterReplays with dynamic filters, pagination, and sorting
        public ObservableCollection<AA_BetterReplays> FilterAndPaginateBetterReplays(
            string teammates, string playlist, string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<AA_BetterReplays>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM AA_BetterReplays WHERE 1 = 1";

                if (!string.IsNullOrEmpty(teammates))
                {
                    command.CommandText += " AND Teammates LIKE @teammates";
                    command.Parameters.AddWithValue("@teammates", $"%{teammates}%");
                }

                if (!string.IsNullOrEmpty(playlist))
                {
                    command.CommandText += " AND Playlist LIKE @playlist";
                    command.Parameters.AddWithValue("@playlist", $"%{playlist}%");
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);

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

        // Method for BetterKillfeed with dynamic filters, pagination, and sorting
        public ObservableCollection<BetterKillfeed> FilterAndPaginateBetterKillfeed(
            string fileName, string actioner, string actionee, string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<BetterKillfeed>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM BetterKillfeed WHERE 1 = 1";

                if (!string.IsNullOrEmpty(fileName))
                {
                    command.CommandText += " AND FileName LIKE @fileName";
                    command.Parameters.AddWithValue("@fileName", $"%{fileName}%");
                }

                if (!string.IsNullOrEmpty(actioner))
                {
                    command.CommandText += " AND Actioner LIKE @actioner";
                    command.Parameters.AddWithValue("@actioner", $"%{actioner}%");
                }

                if (!string.IsNullOrEmpty(actionee))
                {
                    command.CommandText += " AND Actionee LIKE @actionee";
                    command.Parameters.AddWithValue("@actionee", $"%{actionee}%");
                }

                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        replays.Add(new BetterKillfeed
                        {
                            FileName = reader.GetString(reader.GetOrdinal("FileName")),
                            ActionNumber = reader.GetInt32(reader.GetOrdinal("ActionNumber")),
                            Actioner = reader.GetString(reader.GetOrdinal("Actioner")),
                            Team1 = reader.GetInt32(reader.GetOrdinal("Team1")),
                            Actionee = reader.GetString(reader.GetOrdinal("Actionee")),
                            Team2 = reader.GetInt32(reader.GetOrdinal("Team2")),
                            Status = reader.GetString(reader.GetOrdinal("Status")),
                            Action = reader.IsDBNull(reader.GetOrdinal("Action")) ? null : reader.GetString(reader.GetOrdinal("Action")),
                            Rarity = reader.GetString(reader.GetOrdinal("Rarity")),
                            Weapon = reader.GetString(reader.GetOrdinal("Weapon")),
                            POI = reader.IsDBNull(reader.GetOrdinal("POI")) ? null : reader.GetString(reader.GetOrdinal("POI")),
                            ActionTime = reader.GetFloat(reader.GetOrdinal("ActionTime")),
                            ReplayDate = reader.GetDateTime(reader.GetOrdinal("ReplayDate")),
                            ActionerId = reader.GetString(reader.GetOrdinal("ActionerId")),
                            ActioneeId = reader.GetString(reader.GetOrdinal("ActioneeId"))
                        });
                    }
                }
            }

            return replays;
        }

        // Method for A_RosterWithCount with dynamic filters, pagination, and sorting
        public ObservableCollection<A_RosterWithCount> FilterAndPaginateRosterWithCount(
            string fileName, DateTime? replayDate, string displayName, string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var replays = new ObservableCollection<A_RosterWithCount>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "SELECT * FROM A_RosterWithCount WHERE 1 = 1";

                if (!string.IsNullOrEmpty(fileName))
                {
                    command.CommandText += " AND FileName LIKE @fileName";
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

                if (!string.IsNullOrEmpty(orderBy))
                {
                    command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }

                command.CommandText += " LIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber - 1) * pageSize);

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
                            Platform = reader.GetString(reader.GetOrdinal("Platform")),
                            Team = reader.GetInt32(reader.GetOrdinal("Team")),
                            Kills = reader.GetInt32(reader.GetOrdinal("Kills")),
                            BotKills = reader.GetInt32(reader.GetOrdinal("BotKills")),
                            Crowns = reader.GetInt32(reader.GetOrdinal("Crowns")),
                            TeamMate = reader.GetString(reader.GetOrdinal("TeamMate")),
                            Skin = reader.GetString(reader.GetOrdinal("Skin")),
                            Count = reader.GetInt32(reader.GetOrdinal("Count")),
                            MetK = reader.GetString(reader.GetOrdinal("MetK")),
                            MetD = reader.GetString(reader.GetOrdinal("MetD")),
                            Season = reader.GetString(reader.GetOrdinal("Season")),
                            FileName = reader.GetString(reader.GetOrdinal("FileName"))
                        });
                    }
                }
            }

            return replays;
        }
    }
}
