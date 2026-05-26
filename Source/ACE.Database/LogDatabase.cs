using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using log4net;

using ACE.Database.Models.Log;

namespace ACE.Database
{
    public class LogDatabase
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static bool IsConfigured => Common.ConfigManager.Config.MySql.Log != null;

        public bool Exists(bool retryUntilFound)
        {
            if (!IsConfigured)
                return false;

            var config = Common.ConfigManager.Config.MySql.Log;

            for (;;)
            {
                using (var context = new LogDbContext())
                {
                    if (((RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>()).Exists())
                    {
                        log.Debug($"[DATABASE] Successfully connected to {config.Database} database on {config.Host}:{config.Port}.");
                        return true;
                    }
                }

                log.Error($"[DATABASE] Attempting to reconnect to {config.Database} database on {config.Host}:{config.Port} in 5 seconds...");

                if (retryUntilFound)
                    Thread.Sleep(5000);
                else
                    return false;
            }
        }

        #region Account Session Log

        public void LogAccountSessionStart(uint accountId, string accountName, string sessionIP)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO account_session_log (accountId, accountName, sessionIP, loginDateTime)
                            VALUES ({accountId}, {accountName}, {sessionIP}, {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogAccountSessionStart saving session log data to DB. Ex: {ex}");
            }
        }

        public void LogAccountSessionEnd(uint accountId)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"UPDATE account_session_log SET logoutDateTime = {DateTime.Now}
                            WHERE accountId = {accountId} AND logoutDateTime IS NULL;");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogAccountSessionEnd saving session log data to DB for AccountId = {accountId}. Ex: {ex}");
            }
        }

        #endregion

        #region Character Login Log

        public void LogCharacterLogin(uint accountId, string accountName, string sessionIP, uint characterId, string characterName)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO character_login_log (accountId, accountName, characterId, characterName, sessionIP, loginDateTime)
                            VALUES ({accountId}, {accountName}, {characterId}, {characterName}, {sessionIP}, {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogCharacterLogin saving character login info to DB for AccountId = {accountId}, CharacterId = {characterId}. Ex: {ex}");
            }
        }

        public void LogCharacterLogout(uint characterId)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"UPDATE character_login_log SET logoutDateTime = {DateTime.Now}
                            WHERE characterId = {characterId} AND logoutDateTime IS NULL;");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogCharacterLogout saving session log data to DB for CharacterId = {characterId}. Ex: {ex}");
            }
        }

        #endregion

        #region Tinkering Log

        public void LogTinkeringEvent(uint characterId, string characterName, uint itemBiotaId, float chance, float roll, bool isSuccess, uint itemNumPreviousTinks, uint itemWorkmanship, string salvageType, uint salvageWorkmanship)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO tinker_log (characterId, characterName, itemBiotaId, tinkDateTime, successChance, roll, isSuccess, itemNumPreviousTinks, itemWorkmanship, salvageType, salvageWorkmanship)
                            VALUES ({characterId}, {characterName}, {itemBiotaId}, {DateTime.Now}, {chance}, {roll}, {isSuccess}, {itemNumPreviousTinks}, {itemWorkmanship}, {salvageType}, {salvageWorkmanship});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogTinkeringEvent saving data to DB for CharacterId = {characterId}, ItemBiotaId = {itemBiotaId}. Ex: {ex}");
            }
        }

        #endregion

        #region PK Kills Log

        public void LogPkKill(uint victimId, uint killerId, uint? victimMonarchId, uint? killerMonarchId, uint? victimArenaPlayerId = null, uint? killerArenaPlayerId = null)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO pk_kills_log (killer_id, victim_id, killer_monarch_id, victim_monarch_id, kill_datetime, killer_arena_player_id, victim_arena_player_id)
                            VALUES ({killerId}, {victimId}, {killerMonarchId}, {victimMonarchId}, {DateTime.Now}, {killerArenaPlayerId}, {victimArenaPlayerId});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogPkKill saving kill data to DB for KillerId = {killerId}, VictimId = {victimId}. Ex: {ex}");
            }
        }

        #endregion

        #region Arenas

        public uint SaveArenaEvent(ArenaEvent arenaEvent)
        {
            if (!IsConfigured) return 0;
            try
            {
                using (var context = new LogDbContext())
                {
                    if (arenaEvent.Id <= 0)
                        context.ArenaEvents.Add(arenaEvent);
                    else
                        context.Entry(arenaEvent).State = EntityState.Modified;

                    context.SaveChanges();

                    foreach (var arenaPlayer in arenaEvent.Players)
                    {
                        arenaPlayer.EventId = arenaEvent.Id;

                        if (arenaPlayer.Id <= 0)
                            context.ArenaPlayers.Add(arenaPlayer);
                        else
                            context.Entry(arenaPlayer).State = EntityState.Modified;
                    }

                    context.SaveChanges();

                    return arenaEvent.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in SaveArenaEvent. Ex: {ex}");
            }

            return 0;
        }

        /// <summary>
        /// Updates or creates the arena stats row for a character after a match.
        /// </summary>
        /// <param name="newElo">
        /// If set, replaces the stored raw ELO (1v1 / 2v2 only).
        /// Pass null for FFA, Tugak, draw results, or any case where ELO should not change.
        /// </param>
        /// <param name="addRankPoints">
        /// Amount to add to the stored RankPoints column (FFA / Tugak placement points).
        /// Has no effect on the composite score used for 1v1 / 2v2 leaderboards.
        /// </param>
        /// <param name="survived">
        /// Whether to increment TotalSurvived (2v2 survival bonus; always false for other modes).
        /// </param>
        public void AddToArenaStats(
            uint characterId, string characterName, string eventType,
            uint totalMatches, uint totalWins, uint totalDraws, uint totalLosses, uint totalDisqualified,
            uint totalDeaths, uint totalKills, uint totalDmgDealt, uint totalDmgReceived,
            uint? newElo = null, uint addRankPoints = 0, bool survived = false)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    var stats = context.ArenaCharacterStats.FirstOrDefault(x => x.CharacterId == characterId && x.EventType.Equals(eventType));
                    if (stats == null)
                    {
                        stats = new ArenaCharacterStats
                        {
                            CharacterId = characterId,
                            CharacterName = characterName,
                            EventType = eventType,
                            Elo = 1500
                        };
                        context.ArenaCharacterStats.Add(stats);
                    }
                    else
                    {
                        context.Entry(stats).State = EntityState.Modified;
                    }

                    stats.TotalMatches      += totalMatches;
                    stats.TotalWins         += totalWins;
                    stats.TotalDraws        += totalDraws;
                    stats.TotalLosses       += totalLosses;
                    stats.TotalDisqualified += totalDisqualified;
                    stats.TotalDeaths       += totalDeaths;
                    stats.TotalKills        += totalKills;
                    stats.TotalDmgDealt     += totalDmgDealt;
                    stats.TotalDmgReceived  += totalDmgReceived;
                    stats.LastMatchDatetime  = DateTime.Now;
                    stats.LastDecayDatetime  = null; // reset decay grace period on match play

                    if (newElo.HasValue)
                        stats.Elo = newElo.Value;

                    if (addRankPoints > 0)
                        stats.RankPoints += addRankPoints;

                    if (survived)
                        stats.TotalSurvived++;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception saving ArenaCharacterStats. ex: {ex}");
            }
        }

        /// <summary>
        /// Updates or creates the team-pair ranking row for a 2v2 match.
        /// Team members must be supplied in any order; the team key is sorted internally.
        /// </summary>
        public void AddToArenaTeamStats(
            uint charIdA, string charNameA, uint charIdB, string charNameB,
            uint totalMatches, uint totalWins, uint totalDraws, uint totalLosses, uint totalDisqualified,
            uint totalSurvived, uint? newElo = null)
        {
            if (!IsConfigured) return;
            try
            {
                // Canonical key: lower ID first
                var (loId, loName, hiId, hiName) = charIdA <= charIdB
                    ? (charIdA, charNameA, charIdB, charNameB)
                    : (charIdB, charNameB, charIdA, charNameA);
                string teamKey = $"{loId}_{hiId}";

                using (var context = new LogDbContext())
                {
                    var team = context.ArenaTeamStats.FirstOrDefault(x => x.TeamKey == teamKey);
                    if (team == null)
                    {
                        team = new ArenaTeamStats
                        {
                            TeamKey        = teamKey,
                            CharacterIdA   = loId,
                            CharacterNameA = loName,
                            CharacterIdB   = hiId,
                            CharacterNameB = hiName,
                            Elo            = 1500
                        };
                        context.ArenaTeamStats.Add(team);
                    }
                    else
                    {
                        // Keep names current in case of rename
                        team.CharacterNameA = loName;
                        team.CharacterNameB = hiName;
                        context.Entry(team).State = EntityState.Modified;
                    }

                    team.TotalMatches      += totalMatches;
                    team.TotalWins         += totalWins;
                    team.TotalDraws        += totalDraws;
                    team.TotalLosses       += totalLosses;
                    team.TotalDisqualified += totalDisqualified;
                    team.TotalSurvived     += totalSurvived;
                    team.LastMatchDatetime  = DateTime.Now;
                    team.LastDecayDatetime  = null; // reset decay grace period on match play

                    if (newElo.HasValue)
                        team.Elo = newElo.Value;

                    // Keep RankPoints in sync with the composite score for display
                    team.RankPoints = ArenaRanking.ComputeCompositeScore(team);

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception saving ArenaTeamStats. ex: {ex}");
            }
        }

        public ArenaCharacterStats GetCharacterArenaStatsByEvent(uint characterId, string eventType)
        {
            if (!IsConfigured) return null;
            try
            {
                using (var context = new LogDbContext())
                {
                    var stats = context.ArenaCharacterStats.FirstOrDefault(x => x.CharacterId == characterId && x.EventType.Equals(eventType));
                    if (stats != null)
                        stats.CompositeScore = IsEloEventType(eventType)
                            ? ArenaRanking.ComputeCompositeScore(stats)
                            : stats.RankPoints;
                    return stats;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetCharacterArenaStatsByEvent. ex:{ex}");
            }

            return null;
        }

        public string GetArenaStatsByCharacterId(uint characterId, string characterName)
        {
            if (!IsConfigured) return "Log database is not configured.";
            var returnMsg = new System.Text.StringBuilder();

            try
            {
                using (var context = new LogDbContext())
                {
                    var stats = context.ArenaCharacterStats.Where(x => x.CharacterId == characterId)?.ToList() ?? new List<ArenaCharacterStats>();

                    returnMsg.Append($"********* Arena Stats for {characterName} *********\n\n");

                    void AppendStats(ArenaCharacterStats s, string label, bool showRank)
                    {
                        bool isElo = IsEloEventType(s.EventType);
                        uint displayScore = isElo
                            ? ArenaRanking.ComputeCompositeScore(s)
                            : s.RankPoints;

                        returnMsg.Append($"{label}\n");
                        if (showRank)
                        {
                            int rank = isElo ? GetArenaRank(s) : GetArenaRankByPoints(s.EventType, s.RankPoints);
                            returnMsg.Append($"  Rank: {rank.ToString("n0")}\n");
                            if (isElo)
                            {
                                returnMsg.Append($"  Score: {displayScore.ToString("n0")}  (ELO: {s.Elo.ToString("n0")}, Wins: {s.TotalWins.ToString("n0")}, Survived: {s.TotalSurvived.ToString("n0")})\n");
                            }
                            else
                            {
                                returnMsg.Append($"  Points: {displayScore.ToString("n0")}\n");
                            }
                        }
                        returnMsg.Append($"  Matches: {s.TotalMatches.ToString("n0")}\n");
                        returnMsg.Append($"  Wins: {s.TotalWins.ToString("n0")}\n");
                        returnMsg.Append($"  Draws: {s.TotalDraws.ToString("n0")}\n");
                        returnMsg.Append($"  Losses: {s.TotalLosses.ToString("n0")}\n");
                        returnMsg.Append($"  Disqualified: {s.TotalDisqualified.ToString("n0")}\n");
                        returnMsg.Append($"  Kills: {s.TotalKills.ToString("n0")}\n");
                        returnMsg.Append($"  Deaths: {s.TotalDeaths.ToString("n0")}\n");
                        returnMsg.Append($"  Damage Dealt: {s.TotalDmgDealt.ToString("n0")}\n");
                        returnMsg.Append($"  Damage Received: {s.TotalDmgReceived.ToString("n0")}\n\n");
                    }

                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("1v1"))   ?? new ArenaCharacterStats { EventType = "1v1",   Elo = 1500 }, "1v1",   true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("2v2"))   ?? new ArenaCharacterStats { EventType = "2v2",   Elo = 1500 }, "2v2",   true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("ffa"))   ?? new ArenaCharacterStats { EventType = "ffa"   }, "FFA",   true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("tugak")) ?? new ArenaCharacterStats { EventType = "tugak" }, "Tugak", true);
                    AppendStats(stats.FirstOrDefault(x => x.EventType.Equals("group")) ?? new ArenaCharacterStats { EventType = "group" }, "Group", false);

                    returnMsg.Append($"Totals:\n");
                    returnMsg.Append($"  Total Matches: {stats.Sum(x => x.TotalMatches).ToString("n0")}\n");
                    returnMsg.Append($"  Total Wins: {stats.Sum(x => x.TotalWins).ToString("n0")}\n");
                    returnMsg.Append($"  Total Draws: {stats.Sum(x => x.TotalDraws).ToString("n0")}\n");
                    returnMsg.Append($"  Total Losses: {stats.Sum(x => x.TotalLosses).ToString("n0")}\n");
                    returnMsg.Append($"  Total Disqualified: {stats.Sum(x => x.TotalDisqualified).ToString("n0")}\n");
                    returnMsg.Append($"  Total Kills: {stats.Sum(x => x.TotalKills).ToString("n0")}\n");
                    returnMsg.Append($"  Total Deaths: {stats.Sum(x => x.TotalDeaths).ToString("n0")}\n");
                    returnMsg.Append($"  Total Damage Dealt: {stats.Sum(x => x.TotalDmgDealt).ToString("n0")}\n");
                    returnMsg.Append($"  Total Damage Received: {stats.Sum(x => x.TotalDmgReceived).ToString("n0")}\n\n");
                    returnMsg.Append($"*****************************\n");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetArenaStatsByCharacterId for characterId = {characterId}. ex: {ex}");
            }

            return returnMsg.ToString();
        }

        // 1v1 and 2v2 are ELO-based; ffa, tugak, group are not.
        private static bool IsEloEventType(string eventType) =>
            eventType == "1v1" || eventType == "2v2";

        /// <summary>
        /// Returns the player's leaderboard rank for a 1v1 or 2v2 event type.
        /// Loads all rows in memory and computes composite scores with decay.
        /// </summary>
        public int GetArenaRank(ArenaCharacterStats playerStats)
        {
            if (!IsConfigured) return -1;
            try
            {
                uint playerScore = ArenaRanking.ComputeCompositeScore(playerStats);
                using (var context = new LogDbContext())
                {
                    var all = context.ArenaCharacterStats
                        .Where(x => x.EventType.Equals(playerStats.EventType))
                        .ToList();
                    int higher = all.Count(x => ArenaRanking.ComputeCompositeScore(x) > playerScore);
                    return higher + 1;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetArenaRank (ELO). ex:{ex}");
            }
            return -1;
        }

        /// <summary>
        /// Returns the player's leaderboard rank for FFA / Tugak (points-based, SQL-side).
        /// </summary>
        public int GetArenaRankByPoints(string eventType, uint rankPoints)
        {
            if (!IsConfigured) return -1;
            try
            {
                using (var context = new LogDbContext())
                {
                    int higher = context.ArenaCharacterStats
                        .Count(x => x.EventType.Equals(eventType) && x.RankPoints > rankPoints);
                    return higher + 1;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetArenaRankByPoints. ex:{ex}");
            }
            return -1;
        }

        /// <summary>
        /// Returns the top 10 players for the given event type.
        /// For 1v1/2v2, sorts by composite score (with decay) computed in memory.
        /// For FFA/Tugak, sorts by accumulated placement points via SQL.
        /// </summary>
        public List<ArenaCharacterStats> GetArenaTopRankedByEventType(string eventType)
        {
            if (!IsConfigured) return new List<ArenaCharacterStats>();
            try
            {
                using (var context = new LogDbContext())
                {
                    if (IsEloEventType(eventType))
                    {
                        // Load all rows and rank by composite score in memory so
                        // ELO decay is applied correctly.
                        var all = context.ArenaCharacterStats
                            .Where(x => x.EventType.Equals(eventType))
                            .ToList();

                        foreach (var s in all)
                            s.CompositeScore = ArenaRanking.ComputeCompositeScore(s);

                        return all.OrderByDescending(x => x.CompositeScore).Take(10).ToList();
                    }
                    else
                    {
                        // FFA / Tugak: accumulated placement points, sort in SQL.
                        var topTen = context.ArenaCharacterStats
                            .Where(x => x.EventType.Equals(eventType))
                            .OrderByDescending(x => x.RankPoints)
                            .Take(10)
                            .ToList();

                        foreach (var s in topTen)
                            s.CompositeScore = s.RankPoints;

                        return topTen;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetArenaTopRankedByEventType. ex:{ex}");
            }

            return new List<ArenaCharacterStats>();
        }

        /// <summary>
        /// Returns the top 10 2v2 team pairs ranked by composite score (with ELO decay).
        /// </summary>
        public List<ArenaTeamStats> GetArenaTopRankedTeams()
        {
            if (!IsConfigured) return new List<ArenaTeamStats>();
            try
            {
                using (var context = new LogDbContext())
                {
                    var all = context.ArenaTeamStats.ToList();
                    foreach (var t in all)
                        t.CompositeScore = ArenaRanking.ComputeCompositeScore(t);
                    return all.OrderByDescending(x => x.CompositeScore).Take(10).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error in GetArenaTopRankedTeams. ex:{ex}");
            }
            return new List<ArenaTeamStats>();
        }

        /// <summary>
        /// Applies daily ELO decay to all 1v1 and 2v2 character and team stats rows.
        /// Called once per day by ArenaManager.Tick().  Rows still within the
        /// <see cref="ArenaRanking.EloDecayGracePeriodDays"/>-day grace period, or
        /// already at the floor, are skipped.  Changes are persisted so that raw DB
        /// queries always reflect accurate rank scores.
        /// </summary>
        public void ApplyArenaEloDecay()
        {
            if (!IsConfigured) return;
            try
            {
                // ── Individual character stats ────────────────────────────────────
                using (var context = new LogDbContext())
                {
                    var eloRows = context.ArenaCharacterStats
                        .Where(x => x.EventType == "1v1" || x.EventType == "2v2")
                        .ToList();

                    bool anyChanged = false;
                    foreach (var stats in eloRows)
                    {
                        var result = ArenaRanking.ComputeAndApplyDecay(
                            stats.Elo, stats.LastMatchDatetime, stats.LastDecayDatetime);

                        if (!result.HasValue) continue;

                        stats.Elo               = result.Value.newElo;
                        stats.LastDecayDatetime = result.Value.newLastDecayDatetime;
                        anyChanged = true;
                    }

                    if (anyChanged)
                        context.SaveChanges();
                }

                // ── Team stats ────────────────────────────────────────────────────
                using (var context = new LogDbContext())
                {
                    var teamRows = context.ArenaTeamStats.ToList();

                    bool anyChanged = false;
                    foreach (var team in teamRows)
                    {
                        var result = ArenaRanking.ComputeAndApplyDecay(
                            team.Elo, team.LastMatchDatetime, team.LastDecayDatetime);

                        if (!result.HasValue) continue;

                        team.Elo               = result.Value.newElo;
                        team.LastDecayDatetime = result.Value.newLastDecayDatetime;
                        // Keep RankPoints snapshot current for raw DB queries
                        team.RankPoints        = ArenaRanking.ComputeCompositeScore(team);
                        anyChanged = true;
                    }

                    if (anyChanged)
                        context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in ApplyArenaEloDecay. Ex: {ex}");
            }
        }

        public uint CreateArenaPlayer(ArenaPlayer player)
        {
            if (!IsConfigured) return 0;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.ArenaPlayers.Add(player);
                    context.SaveChanges();
                    return player.Id;
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in CreateArenaPlayer. Ex: {ex}");
            }

            return 0;
        }

        public void UpdateArenaPlayer(ArenaPlayer player)
        {
            if (!IsConfigured) return;
            using (var context = new LogDbContext())
            {
                context.Entry(player).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public List<ArenaEvent> GetAllArenaEvents()
        {
            if (!IsConfigured) return new List<ArenaEvent>();
            List<ArenaEvent> eventList = null;

            try
            {
                using (var context = new LogDbContext())
                {
                    eventList = context.ArenaEvents
                        .AsNoTracking()
                        .OrderByDescending(r => r.StartDateTime)
                        .Where(r => r.EndDateTime.HasValue)
                        ?.ToList() ?? new List<ArenaEvent>();
                }

                foreach (var arenaEvent in eventList)
                    arenaEvent.Players = GetAllArenaPlayersByEvent(arenaEvent.Id);
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetAllArenaEvents. Ex: {ex}");
            }

            return eventList ?? new List<ArenaEvent>();
        }

        public List<ArenaPlayer> GetAllArenaPlayersByEvent(uint eventId)
        {
            if (!IsConfigured) return new List<ArenaPlayer>();
            List<ArenaPlayer> playerList = null;

            try
            {
                using (var context = new LogDbContext())
                {
                    playerList = context.ArenaPlayers
                        .AsNoTracking()
                        .Where(x => x.EventId == (uint?)eventId)
                        ?.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetAllArenaPlayersByEvent. Ex: {ex}");
            }

            return playerList ?? new List<ArenaPlayer>();
        }

        #endregion

        #region Rare Log

        public void LogRare(RareLog rareLog)
        {
            if (!IsConfigured || rareLog == null) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO rare_log (characterName, characterId, itemName, itemBiotaId, itemWeenieId, createDateTime)
                            VALUES ({rareLog.CharacterName}, {rareLog.CharacterId}, {rareLog.ItemName}, {rareLog.ItemBiotaId}, {rareLog.ItemWeenieId}, {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogRare saving rare event to DB. ex: {ex}");
            }
        }

        #endregion

        #region Stuck Character Log

        public void LogStuckCharacter(StuckCharacterLog stuckLog)
        {
            if (!IsConfigured || stuckLog == null) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.Database.ExecuteSql(
                        @$"INSERT INTO stuck_character_log
                        (playerGuid, playerName, accountName, accountId, sessionInfo, landblock, location,
                         isLoggingOut, isInDeathProcess, foundOnLandblock, forcedLogOffRequested,
                         pkLogoutState, materializedLogoutState, logoffPath, createdAtUtc)
                        VALUES
                        ({stuckLog.PlayerGuid},
                         {stuckLog.PlayerName ?? (object)DBNull.Value},
                         {stuckLog.AccountName ?? (object)DBNull.Value},
                         {stuckLog.AccountId},
                         {stuckLog.SessionInfo ?? (object)DBNull.Value},
                         {stuckLog.Landblock ?? (object)DBNull.Value},
                         {stuckLog.Location ?? (object)DBNull.Value},
                         {stuckLog.IsLoggingOut},
                         {stuckLog.IsInDeathProcess},
                         {stuckLog.FoundOnLandblock},
                         {stuckLog.ForcedLogOffRequested},
                         {stuckLog.PkLogoutState},
                         {stuckLog.MaterializedLogoutState},
                         {stuckLog.LogoffPath ?? (object)DBNull.Value},
                         {DateTime.Now});");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogStuckCharacter saving stuck character event to DB. ex: {ex}");
            }
        }

        #endregion

        #region Movement Violation

        /// <summary>
        /// Records a single movement anti-cheat violation event for long-term ban evidence.
        /// Fire-and-forget: failures are logged but never thrown to the caller.
        /// </summary>
        /// <param name="violationType">
        /// Short identifier for the check that fired, e.g. "speed_packet", "script_timing",
        /// "geometry", "door_ghost".  See MovementViolationLog.ViolationType for the full list.
        /// </param>
        public void LogMovementViolation(uint characterId, string characterName, string accountName,
            string violationType, float observedSpeed, float allowedSpeed, float suspicionScore, string location)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    context.MovementViolationLogs.Add(new MovementViolationLog
                    {
                        CharacterId       = characterId,
                        CharacterName     = characterName,
                        AccountName       = accountName,
                        ViolationType     = violationType ?? "unknown",
                        ObservedSpeed     = observedSpeed,
                        AllowedSpeed      = allowedSpeed,
                        SuspicionScore    = suspicionScore,
                        Location          = location,
                        ViolationDateTime = DateTime.UtcNow
                    });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in LogMovementViolation for character {characterName} ({characterId}). Ex: {ex}");
            }
        }

        #endregion

        #region Town Control

        public List<TownControlTown> GetAllTownControlTowns()
        {
            if (!IsConfigured) return new List<TownControlTown>();
            try
            {
                using (var context = new LogDbContext())
                {
                    return context.TownControlTowns.AsNoTracking().ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetAllTownControlTowns. Ex: {ex}");
            }
            return new List<TownControlTown>();
        }

        public void UpdateTownControlTown(TownControlTown town)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    var rec = context.TownControlTowns.FirstOrDefault(x => x.TownId == town.TownId);
                    if (rec == null) return;

                    rec.OwnerId                = town.OwnerId;
                    rec.IsInConflict           = town.IsInConflict;
                    rec.LastConflictStartTime  = town.LastConflictStartTime;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in UpdateTownControlTown for TownId = {town.TownId}. Ex: {ex}");
            }
        }

        /// <summary>
        /// Inserts a new town_control_event row and returns the populated record (with auto-assigned EventId).
        /// </summary>
        public TownControlEvent StartTownControlEvent(ushort townId, uint attackerId, string attackerClanName,
            uint? defenderId, string defenderClanName)
        {
            if (!IsConfigured) return null;
            try
            {
                var tcEvent = new TownControlEvent
                {
                    TownId           = townId,
                    AttackerId       = attackerId,
                    AttackerClanName = attackerClanName,
                    DefenderId       = defenderId,
                    DefenderClanName = defenderClanName,
                    EventStartTime   = DateTime.UtcNow,
                    EventEndTime     = null,
                    IsAttackSuccess  = null
                };

                using (var context = new LogDbContext())
                {
                    context.TownControlEvents.Add(tcEvent);
                    context.SaveChanges();
                }

                return tcEvent;
            }
            catch (Exception ex)
            {
                log.Error($"Exception in StartTownControlEvent for TownId = {townId}. Ex: {ex}");
            }
            return null;
        }

        public void UpdateTownControlEvent(TownControlEvent tcEvent)
        {
            if (!IsConfigured) return;
            try
            {
                using (var context = new LogDbContext())
                {
                    var rec = context.TownControlEvents.FirstOrDefault(x => x.EventId == tcEvent.EventId);
                    if (rec == null) return;

                    rec.AttackerId       = tcEvent.AttackerId;
                    rec.AttackerClanName = tcEvent.AttackerClanName;
                    rec.DefenderId       = tcEvent.DefenderId;
                    rec.DefenderClanName = tcEvent.DefenderClanName;
                    rec.EventStartTime   = tcEvent.EventStartTime;
                    rec.EventEndTime     = tcEvent.EventEndTime;
                    rec.IsAttackSuccess  = tcEvent.IsAttackSuccess;

                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in UpdateTownControlEvent for EventId = {tcEvent.EventId}. Ex: {ex}");
            }
        }

        /// <summary>
        /// Returns the most recent event for a given town (by EventId descending), or null if none.
        /// </summary>
        public TownControlEvent GetLatestTownControlEventByTownId(ushort townId)
        {
            if (!IsConfigured) return null;
            try
            {
                using (var context = new LogDbContext())
                {
                    return context.TownControlEvents
                        .AsNoTracking()
                        .Where(r => r.TownId == townId)
                        .OrderByDescending(r => r.EventId)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetLatestTownControlEventByTownId for TownId = {townId}. Ex: {ex}");
            }
            return null;
        }

        /// <summary>
        /// Returns the most recent event for a given town where a specific allegiance was the attacker.
        /// Used to enforce respite timers.
        /// </summary>
        public TownControlEvent GetLatestTownControlEventByAttacker(uint attackerId, ushort townId)
        {
            if (!IsConfigured) return null;
            try
            {
                using (var context = new LogDbContext())
                {
                    return context.TownControlEvents
                        .AsNoTracking()
                        .Where(r => r.TownId == townId && r.AttackerId == attackerId)
                        .OrderByDescending(r => r.EventId)
                        .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Exception in GetLatestTownControlEventByAttacker for AttackerId = {attackerId}, TownId = {townId}. Ex: {ex}");
            }
            return null;
        }

        #endregion
    }
}
