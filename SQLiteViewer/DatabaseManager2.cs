﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;

namespace SQLiteViewer
{
    public class DatabaseManager
    {
        private readonly string connectionString;

        public DatabaseManager(string dbPath)
        {
            connectionString = $"Data Source={dbPath}";
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
    }
}