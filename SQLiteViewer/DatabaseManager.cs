using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Data.Common;
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

                string query = $"PRAGMA cache_size = -100000; PRAGMA synchronous = OFF; SELECT COUNT(*) FROM {tableName}";

                using (var command = new SqliteCommand(query, connection))
                {
                    rowCount = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return rowCount;
        }
        public (ObservableCollection<AA_BetterReplays>,int) FilterAndPaginateBetterReplays(
            string teammate, string playlist,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var betterreplaycommand = "PRAGMA cache_size = -1000000; PRAGMA synchronous = OFF; SELECT \r\n    R.\"fileName\", \r\n    R.\"replayDate\", \r\n    R.\"playlist\",\r\n\trt.teammates,\r\n    R.\"gameTime\", \r\n    R.\"season\", \r\n    COUNT(CASE WHEN ro.\"isBot\" > 0 THEN 1 END) AS \"botCount\",\r\n    SUM(CASE WHEN ro.\"replayPlayer\" > 0 THEN ro.\"kills\" ELSE 0 END) AS \"Kills\",\r\n    BK.bot_kills AS BotKills, -- Joining the bot_kills column from BotKills view\r\n    MAX(CASE WHEN ro.\"replayPlayer\" > 0 THEN ro.\"placement\" END) AS \"Placement\",\r\n    (SELECT POI\r\n     FROM BetterKillfeed\r\n     WHERE BetterKillfeed.fileName = R.fileName\r\n       AND POI IS NOT NULL\r\n     ORDER BY '#' DESC\r\n     LIMIT 1) AS \"Ended\" \r\nFROM \r\n    \"Replays\" R\r\nLEFT JOIN \r\n    \"Roster\" ro ON R.\"fileName\" = ro.\"fileName\"\r\nLEFT JOIN \r\n    BotKills BK ON R.\"fileName\" = BK.\"fileName\" AND BK.replayPlayer > 0\r\nLEFT JOIN\r\n    rTeammates rt ON R.\"fileName\" = rt.\"fileName\"\r\nWHERECommandC# \r\nGROUP BY \r\n    R.\"fileName\", R.\"replayDate\", R.\"playlist\", R.\"gameTime\", R.\"season\", rt.teammates";
            var total_rows = 0;
            var replays = new ObservableCollection<AA_BetterReplays>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "WHERE 1 = 1";
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


                
                var command2 = connection.CreateCommand();
                command2.CommandText = betterreplaycommand.Replace("WHERECommandC#", command.CommandText);
                command2.CommandText = command2.CommandText.Replace("    R.\"fileName\", \r\n    R.\"replayDate\", \r\n    R.\"playlist\",\r\n\trt.teammates,\r\n    R.\"gameTime\", \r\n    R.\"season\", \r\n    COUNT(CASE WHEN ro.\"isBot\" > 0 THEN 1 END) AS \"botCount\",\r\n    SUM(CASE WHEN ro.\"replayPlayer\" > 0 THEN ro.\"kills\" ELSE 0 END) AS \"Kills\",\r\n    BK.bot_kills AS BotKills, -- Joining the bot_kills column from BotKills view\r\n    MAX(CASE WHEN ro.\"replayPlayer\" > 0 THEN ro.\"placement\" END) AS \"Placement\",\r\n    (SELECT POI\r\n     FROM BetterKillfeed\r\n     WHERE BetterKillfeed.fileName = R.fileName\r\n       AND POI IS NOT NULL\r\n     ORDER BY '#' DESC\r\n     LIMIT 1) AS \"Ended\" ", "COUNT(*)");
                command2.CommandType = command.CommandType;
                command2.Transaction = command.Transaction;
                foreach (SqliteParameter parameter in command.Parameters)
                {
                    var newParameter = new SqliteParameter
                    {
                        ParameterName = parameter.ParameterName,
                        Value = parameter.Value,
                        DbType = parameter.DbType,
                        Direction = parameter.Direction,
                        Size = parameter.Size,
                        IsNullable = parameter.IsNullable
                    };

                    command2.Parameters.Add(newParameter);
                }
                var resutl = command2.ExecuteScalar();
                if (resutl != null)
                {
                    total_rows = int.Parse(resutl.ToString());
                }

                // Sorting
                if (!string.IsNullOrEmpty(orderBy))
                {

                    //command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                    if (orderBy== "FileName")
                    {
                        orderBy = "R.FileName";
                    }
                    betterreplaycommand += $"\r\nORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";

                }

                // Pagination
                //command.CommandText += " LIMIT @pageSize OFFSET @offset";
                betterreplaycommand += "\r\nLIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber) * pageSize);
                command.CommandText = betterreplaycommand.Replace("WHERECommandC#", command.CommandText);

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

            return (replays,total_rows);
        }
        public (ObservableCollection<AA_DistinctRoster>, int) FilterAndPaginateDistinctRoster(
            string fileName, DateTime? replayDate, string displayName, int? minLvl, int? maxLvl,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var total_rows = 0;
            var replays = new ObservableCollection<AA_DistinctRoster>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "PRAGMA cache_size = -1000000; PRAGMA synchronous = OFF; SELECT * FROM AA_DistinctRoster WHERE 1 = 1";

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
                var command2 = connection.CreateCommand();
                command2.CommandText = command.CommandText.Replace("*", "COUNT(*)");
                command2.CommandType = command.CommandType;
                command2.Transaction = command.Transaction;
                foreach (SqliteParameter parameter in command.Parameters)
                {
                    var newParameter = new SqliteParameter
                    {
                        ParameterName = parameter.ParameterName,
                        Value = parameter.Value,
                        DbType = parameter.DbType,
                        Direction = parameter.Direction,
                        Size = parameter.Size,
                        IsNullable = parameter.IsNullable
                    };

                    command2.Parameters.Add(newParameter);
                }
                var resutl = command2.ExecuteScalar();
                if (resutl != null)
                {
                    total_rows = int.Parse(resutl.ToString());
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

            return (replays,total_rows);
        }
        public (ObservableCollection<A_RosterWithCount>,int) FilterAndPaginateRosterWithCount(
            string fileName, DateTime? replayDate, string displayName, string isBot, string isTeam, string isAnon,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            string roastercommand = "PRAGMA cache_size = -1000000; PRAGMA synchronous = OFF; SELECT \r\n    RV.item_number AS Num,  -- Adding the item_number column from the zReplayView\r\n    A.replayDate AS Date, \r\n    A.playlist,\r\n    LR.playerId,\r\n    LR.displayName,\r\n    LR.level AS Lvl,\r\n    LR.placement AS Place, \r\n    LR.anonymous AS Anon,\r\n    LR.platform,\r\n    LR.teamIndex AS Team,\r\n    LR.kills AS Kills,\r\n    BK.bot_kills AS BotKills,\r\n    LR.crowns,\r\n    COALESCE(\r\n        (\r\n            SELECT displayName\r\n            FROM (\r\n                SELECT \r\n                    displayName,\r\n                    ROW_NUMBER() OVER (PARTITION BY LR.fileName, LR.teamIndex ORDER BY placement DESC) AS rn\r\n                FROM Roster\r\n                WHERE LR.fileName = fileName\r\n                  AND LR.teamIndex = teamIndex\r\n                  AND displayName <> LR.displayName\r\n            ) AS Subquery\r\n            WHERE rn = 1\r\n        ), 'No Teammate'\r\n    ) AS TeamMate,\r\n\tLR.isBot,\r\n    LR.skin, \r\n    (\r\n        SELECT COUNT(*)\r\n        FROM Roster\r\n        WHERE \r\n            playerId = LR.playerId \r\n            AND isBot < 1\r\n    ) AS Count,\r\n    \r\n    SUM(\r\n        CASE \r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actionerId AND tm.fileName = kf.fileName\r\n                WHERE kf.actioneeId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer = 1\r\n            ) THEN 1\r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actionerId AND tm.fileName = kf.fileName\r\n                WHERE kf.actioneeId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer != 1\r\n            ) THEN 2\r\n            ELSE 0\r\n        END\r\n    ) AS MetK,\r\n    \r\n    SUM(\r\n        CASE \r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actioneeId AND tm.fileName = kf.fileName\r\n                WHERE kf.actionerId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer = 1\r\n            ) THEN 1\r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actioneeId AND tm.fileName = kf.fileName\r\n                WHERE kf.actionerId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer != 1\r\n            ) THEN 2\r\n            ELSE 0\r\n        END\r\n    ) AS MetD,\r\n    \r\n    -- Adding the new is_team column\r\n    CASE\r\n        WHEN EXISTS (\r\n            SELECT 1\r\n            FROM Teammates tm\r\n            WHERE tm.playerId = LR.playerId\r\n              AND tm.fileName = LR.fileName\r\n        ) THEN 1\r\n        ELSE 0\r\n    END AS isTeam,\r\n    \r\n    A.season,\r\n    LR.fileName AS fileName\r\nFROM \r\n    Roster LR\r\nJOIN \r\n    Replays A ON LR.fileName = A.fileName\r\nLEFT JOIN \r\n    BotKills BK ON LR.fileName = BK.fileName AND LR.playerId = BK.actionerId\r\nJOIN \r\n    zReplayView RV ON LR.fileName = RV.fileName\r\n WHERECommandC# \r\nGROUP BY \r\n    A.replayDate, A.playlist, LR.displayName, LR.kills, LR.level, LR.placement, LR.anonymous, LR.teamIndex, LR.isBot, LR.crowns, LR.skin, RV.item_number";
            var total_rows = 0;
            var replays = new ObservableCollection<A_RosterWithCount>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "WHERE 1 = 1";

                // Add filters dynamically
                if (!string.IsNullOrEmpty(fileName))
                {
                    command.CommandText += " AND LR.fileName LIKE @fileName";
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
                if (!string.IsNullOrEmpty(isAnon))
                {
                    command.CommandText += $" AND Anon = {isAnon}";
                }

                var command2 = connection.CreateCommand();
                command2.CommandText = roastercommand.Replace("WHERECommandC#", command.CommandText);
                command2.CommandText = command2.CommandText.Replace("    RV.item_number AS Num,  -- Adding the item_number column from the zReplayView\r\n    A.replayDate AS Date, \r\n    A.playlist,\r\n    LR.playerId,\r\n    LR.displayName,\r\n    LR.level AS Lvl,\r\n    LR.placement AS Place, \r\n    LR.anonymous AS Anon,\r\n    LR.platform,\r\n    LR.teamIndex AS Team,\r\n    LR.kills AS Kills,\r\n    BK.bot_kills AS BotKills,\r\n    LR.crowns,\r\n    COALESCE(\r\n        (\r\n            SELECT displayName\r\n            FROM (\r\n                SELECT \r\n                    displayName,\r\n                    ROW_NUMBER() OVER (PARTITION BY LR.fileName, LR.teamIndex ORDER BY placement DESC) AS rn\r\n                FROM Roster\r\n                WHERE LR.fileName = fileName\r\n                  AND LR.teamIndex = teamIndex\r\n                  AND displayName <> LR.displayName\r\n            ) AS Subquery\r\n            WHERE rn = 1\r\n        ), 'No Teammate'\r\n    ) AS TeamMate,\r\n\tLR.isBot,\r\n    LR.skin, \r\n    (\r\n        SELECT COUNT(*)\r\n        FROM Roster\r\n        WHERE \r\n            playerId = LR.playerId \r\n            AND isBot < 1\r\n    ) AS Count,\r\n    \r\n    SUM(\r\n        CASE \r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actionerId AND tm.fileName = kf.fileName\r\n                WHERE kf.actioneeId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer = 1\r\n            ) THEN 1\r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actionerId AND tm.fileName = kf.fileName\r\n                WHERE kf.actioneeId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer != 1\r\n            ) THEN 2\r\n            ELSE 0\r\n        END\r\n    ) AS MetK,\r\n    \r\n    SUM(\r\n        CASE \r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actioneeId AND tm.fileName = kf.fileName\r\n                WHERE kf.actionerId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer = 1\r\n            ) THEN 1\r\n            WHEN EXISTS (\r\n                SELECT 1\r\n                FROM Killfeed kf\r\n                JOIN Teammates tm ON tm.playerId = kf.actioneeId AND tm.fileName = kf.fileName\r\n                WHERE kf.actionerId = LR.playerId\r\n                  AND kf.fileName = LR.fileName\r\n                  AND tm.replayPlayer != 1\r\n            ) THEN 2\r\n            ELSE 0\r\n        END\r\n    ) AS MetD,\r\n    \r\n    -- Adding the new is_team column\r\n    CASE\r\n        WHEN EXISTS (\r\n            SELECT 1\r\n            FROM Teammates tm\r\n            WHERE tm.playerId = LR.playerId\r\n              AND tm.fileName = LR.fileName\r\n        ) THEN 1\r\n        ELSE 0\r\n    END AS isTeam,\r\n    \r\n    A.season,\r\n    LR.fileName AS fileName", "    RV.item_number AS Num,  -- Adding the item_number column from the zReplayView\r\n    A.replayDate AS Date, \r\n    LR.level AS Lvl,\r\n    LR.placement AS Place, \r\n    LR.anonymous AS Anon,\r\n    LR.teamIndex AS Team,\r\n    LR.kills AS Kills,\r\n    BK.bot_kills AS BotKills,\r\n\tLR.isBot,\r\n    LR.skin, \r\n    CASE\r\n        WHEN EXISTS (\r\n            SELECT 1\r\n            FROM Teammates tm\r\n            WHERE tm.playerId = LR.playerId\r\n              AND tm.fileName = LR.fileName\r\n        ) THEN 1\r\n        ELSE 0\r\n    END AS isTeam,\r\n    A.season,\r\n    LR.fileName AS fileName");
                command2.CommandType = command.CommandType;
                command2.Transaction = command.Transaction;
                foreach (SqliteParameter parameter in command.Parameters)
                {
                    var newParameter = new SqliteParameter
                    {
                        ParameterName = parameter.ParameterName,
                        Value = parameter.Value,
                        DbType = parameter.DbType,
                        Direction = parameter.Direction,
                        Size = parameter.Size,
                        IsNullable = parameter.IsNullable
                    };

                    command2.Parameters.Add(newParameter);
                }
                var result = command2.ExecuteReader();
                while (result.Read())
                {
                    total_rows++;
                }

                result.Close();

                // Sorting
                if (!string.IsNullOrEmpty(orderBy))
                {
                    //command.CommandText += $" ORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                    if (orderBy == "FileName")
                    {
                        orderBy = "LR.FileName";
                    }
                    roastercommand += $"\r\nORDER BY {orderBy} {(ascending ? "ASC" : "DESC")}";
                }
                // Pagination
                //command.CommandText += " LIMIT @pageSize OFFSET @offset";
                roastercommand += "\r\nLIMIT @pageSize OFFSET @offset";
                command.Parameters.AddWithValue("@pageSize", pageSize);
                command.Parameters.AddWithValue("@offset", (pageNumber) * pageSize);
                command.CommandText = roastercommand.Replace("WHERECommandC#", command.CommandText);
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

            return (replays, total_rows);
        }
        public (ObservableCollection<BetterKillfeed>,int) FilterAndPaginateBetterKillFeed(
            string fileName, string actioner, string actionee,
            string orderBy, bool ascending, int pageNumber, int pageSize)
        {
            var total_rows = 0;
            var replays = new ObservableCollection<BetterKillfeed>();
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                // Base query
                command.CommandText = "PRAGMA cache_size = -1000000; PRAGMA synchronous = OFF; SELECT * FROM BetterKillfeed WHERE 1 = 1";

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
                var command2 = connection.CreateCommand();
                command2.CommandText = command.CommandText.Replace("*", "COUNT(*)");
                command2.CommandType = command.CommandType;
                command2.Transaction = command.Transaction;
                foreach (SqliteParameter parameter in command.Parameters)
                {
                    var newParameter = new SqliteParameter
                    {
                        ParameterName = parameter.ParameterName,
                        Value = parameter.Value,
                        DbType = parameter.DbType,
                        Direction = parameter.Direction,
                        Size = parameter.Size,
                        IsNullable = parameter.IsNullable
                    };

                    command2.Parameters.Add(newParameter);
                }
                var resutl = command2.ExecuteScalar();
                if (resutl != null)
                {
                    total_rows = int.Parse(resutl.ToString());
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

            return (replays,total_rows);
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