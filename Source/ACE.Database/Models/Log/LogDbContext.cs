using Microsoft.EntityFrameworkCore;

namespace ACE.Database.Models.Log
{
    public partial class LogDbContext : DbContext
    {
        public LogDbContext() { }

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options) { }

        public virtual DbSet<PKKill> PKKills { get; set; }
        public virtual DbSet<AccountSessionLog> AccountSessions { get; set; }
        public virtual DbSet<CharacterLoginLog> CharacterLogins { get; set; }
        public virtual DbSet<TinkerLog> TinkeringEvents { get; set; }
        public virtual DbSet<ArenaEvent> ArenaEvents { get; set; }
        public virtual DbSet<ArenaPlayer> ArenaPlayers { get; set; }
        public virtual DbSet<ArenaCharacterStats> ArenaCharacterStats { get; set; }
        public virtual DbSet<ArenaTeamStats> ArenaTeamStats { get; set; }
        public virtual DbSet<RareLog> RareLogs { get; set; }
        public virtual DbSet<StuckCharacterLog> StuckCharacterLogs { get; set; }
        public virtual DbSet<AllegianceHometownTown>       AllegianceHometownTowns      { get; set; }
        public virtual DbSet<AllegianceHometownEvent>      AllegianceHometownEvents     { get; set; }
        public virtual DbSet<AllegianceHometownBlacklist>  AllegianceHometownBlacklists { get; set; }
        public virtual DbSet<MovementViolationLog> MovementViolationLogs { get; set; }

        public virtual DbSet<SeasonCharacterStats>  SeasonCharacterStats   { get; set; }
        public virtual DbSet<SeasonMilestone>       SeasonMilestones       { get; set; }
        public virtual DbSet<SeasonMilestoneLeader> SeasonMilestoneLeaders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = Common.ConfigManager.Config.MySql.Log;

                var connectionString = $"server={config.Host};port={config.Port};user={config.Username};password={config.Password};database={config.Database};TreatTinyAsBoolean=False;SslMode=None;AllowPublicKeyRetrieval=true;ApplicationName=ACEmulator";

                optionsBuilder
                    .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), builder =>
                    {
                        builder.EnableRetryOnFailure(10);
                    });
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            modelBuilder.Entity<TinkerLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("tinker_log");
                entity.Property(e => e.Id).HasColumnName("tinkerLogId");
                entity.Property(e => e.CharacterId).HasColumnName("characterId");
                entity.Property(e => e.CharacterName).HasColumnName("characterName");
                entity.Property(e => e.ItemBiotaId).HasColumnName("itemBiotaId");
                entity.Property(e => e.TinkDateTime).HasColumnName("tinkDateTime");
                entity.Property(e => e.SuccessChance).HasColumnName("successChance");
                entity.Property(e => e.Roll).HasColumnName("roll");
                entity.Property(e => e.IsSuccess).HasColumnName("isSuccess");
                entity.Property(e => e.ItemNumPreviousTinks).HasColumnName("itemNumPreviousTinks");
                entity.Property(e => e.ItemWorkmanship).HasColumnName("itemWorkmanship");
                entity.Property(e => e.SalvageType).HasColumnName("salvageType");
                entity.Property(e => e.SalvageWorkmanship).HasColumnName("salvageWorkmanship");
            });

            modelBuilder.Entity<AccountSessionLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("account_session_log");
                entity.Property(e => e.Id).HasColumnName("sessionLogId");
                entity.Property(e => e.AccountId).HasColumnName("accountId");
                entity.Property(e => e.AccountName).HasColumnName("accountName");
                entity.Property(e => e.SessionIP).HasColumnName("sessionIP");
                entity.Property(e => e.LoginDateTime).HasColumnName("loginDateTime");
                entity.Property(e => e.LogoutDateTime).HasColumnName("logoutDateTime");
            });

            modelBuilder.Entity<CharacterLoginLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("character_login_log");
                entity.Property(e => e.Id).HasColumnName("characterLoginLogId");
                entity.Property(e => e.AccountId).HasColumnName("accountId");
                entity.Property(e => e.AccountName).HasColumnName("accountName");
                entity.Property(e => e.SessionIP).HasColumnName("sessionIP");
                entity.Property(e => e.CharacterId).HasColumnName("characterId");
                entity.Property(e => e.CharacterName).HasColumnName("characterName");
                entity.Property(e => e.LoginDateTime).HasColumnName("loginDateTime");
                entity.Property(e => e.LogoutDateTime).HasColumnName("logoutDateTime");
            });

            modelBuilder.Entity<PKKill>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("pk_kills_log");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.VictimId).HasColumnName("victim_id");
                entity.Property(e => e.KillerId).HasColumnName("killer_id");
                entity.Property(e => e.VictimMonarchId).HasColumnName("victim_monarch_id");
                entity.Property(e => e.KillerMonarchId).HasColumnName("killer_monarch_id");
                entity.Property(e => e.KillDateTime).HasColumnName("kill_datetime");
                entity.Property(e => e.VictimArenaPlayerID).HasColumnName("victim_arena_player_id");
                entity.Property(e => e.KillerArenaPlayerID).HasColumnName("killer_arena_player_id");
            });

            modelBuilder.Entity<ArenaEvent>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("arena_event");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.EventType).HasColumnName("event_type");
                entity.Property(e => e.Status).HasColumnName("status");
                entity.Property(e => e.Location).HasColumnName("location");
                entity.Property(e => e.StartDateTime).HasColumnName("start_datetime");
                entity.Property(e => e.EndDateTime).HasColumnName("end_datetime");
                entity.Property(e => e.WinningTeamGuid).HasColumnName("winning_team_guid");
                entity.Property(e => e.CancelReason).HasColumnName("cancel_reason");
                entity.Property(e => e.IsOvertime).HasColumnName("is_overtime");
                entity.Property(e => e.CreatedDateTime).HasColumnName("create_datetime");
            });

            modelBuilder.Entity<ArenaPlayer>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("arena_player");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CharacterId).HasColumnName("character_id");
                entity.Property(e => e.CharacterName).HasColumnName("character_name");
                entity.Property(e => e.CharacterLevel).HasColumnName("character_level");
                entity.Property(e => e.EventType).HasColumnName("event_type");
                entity.Property(e => e.MonarchId).HasColumnName("monarch_id");
                entity.Property(e => e.MonarchName).HasColumnName("monarch_name");
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.TeamGuid).HasColumnName("team_guid");
                entity.Property(e => e.PlayerIP).HasColumnName("player_ip");
                entity.Property(e => e.IsEliminated).HasColumnName("is_eliminated");
                entity.Property(e => e.FinishPlace).HasColumnName("finish_place");
                entity.Property(e => e.TotalDeaths).HasColumnName("total_deaths");
                entity.Property(e => e.TotalKills).HasColumnName("total_kills");
                entity.Property(e => e.TotalDmgDealt).HasColumnName("total_dmg_dealt");
                entity.Property(e => e.TotalDmgReceived).HasColumnName("total_dmg_received");
                entity.Property(e => e.CreateDateTime).HasColumnName("create_datetime");
            });

            modelBuilder.Entity<ArenaCharacterStats>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("arena_character_stats");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CharacterId).HasColumnName("character_id");
                entity.Property(e => e.CharacterName).HasColumnName("character_name");
                entity.Property(e => e.EventType).HasColumnName("event_type");
                entity.Property(e => e.Elo).HasColumnName("elo");
                entity.Property(e => e.RankPoints).HasColumnName("rank_points");
                entity.Property(e => e.LastMatchDatetime).HasColumnName("last_match_datetime");
                entity.Property(e => e.LastDecayDatetime).HasColumnName("last_decay_datetime");
                entity.Property(e => e.TotalSurvived).HasColumnName("total_survived");
                entity.Property(e => e.TotalMatches).HasColumnName("total_matches");
                entity.Property(e => e.TotalWins).HasColumnName("total_wins");
                entity.Property(e => e.TotalLosses).HasColumnName("total_losses");
                entity.Property(e => e.TotalDraws).HasColumnName("total_draws");
                entity.Property(e => e.TotalDisqualified).HasColumnName("total_disqualified");
                entity.Property(e => e.TotalDeaths).HasColumnName("total_deaths");
                entity.Property(e => e.TotalKills).HasColumnName("total_kills");
                entity.Property(e => e.TotalDmgDealt).HasColumnName("total_dmg_dealt");
                entity.Property(e => e.TotalDmgReceived).HasColumnName("total_dmg_received");
            });

            modelBuilder.Entity<ArenaTeamStats>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("arena_team_stats");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TeamKey).HasColumnName("team_key").HasMaxLength(40);
                entity.Property(e => e.CharacterIdA).HasColumnName("character_id_a");
                entity.Property(e => e.CharacterNameA).HasColumnName("character_name_a");
                entity.Property(e => e.CharacterIdB).HasColumnName("character_id_b");
                entity.Property(e => e.CharacterNameB).HasColumnName("character_name_b");
                entity.Property(e => e.Elo).HasColumnName("elo");
                entity.Property(e => e.RankPoints).HasColumnName("rank_points");
                entity.Property(e => e.TotalMatches).HasColumnName("total_matches");
                entity.Property(e => e.TotalWins).HasColumnName("total_wins");
                entity.Property(e => e.TotalLosses).HasColumnName("total_losses");
                entity.Property(e => e.TotalDraws).HasColumnName("total_draws");
                entity.Property(e => e.TotalDisqualified).HasColumnName("total_disqualified");
                entity.Property(e => e.TotalSurvived).HasColumnName("total_survived");
                entity.Property(e => e.LastMatchDatetime).HasColumnName("last_match_datetime");
                entity.Property(e => e.LastDecayDatetime).HasColumnName("last_decay_datetime");
            });

            modelBuilder.Entity<RareLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("rare_log");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CharacterId).HasColumnName("characterId");
                entity.Property(e => e.CharacterName).HasColumnName("characterName");
                entity.Property(e => e.ItemBiotaId).HasColumnName("itemBiotaId");
                entity.Property(e => e.ItemWeenieId).HasColumnName("itemWeenieId");
                entity.Property(e => e.ItemName).HasColumnName("itemName");
                entity.Property(e => e.CreatedDateTime).HasColumnName("createDateTime");
            });

            modelBuilder.Entity<StuckCharacterLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("stuck_character_log");
                entity.Property(e => e.Id).HasColumnName("stuckCharacterLogId");
                entity.Property(e => e.PlayerGuid).HasColumnName("playerGuid");
                entity.Property(e => e.PlayerName).HasColumnName("playerName");
                entity.Property(e => e.AccountName).HasColumnName("accountName");
                entity.Property(e => e.AccountId).HasColumnName("accountId");
                entity.Property(e => e.SessionInfo).HasColumnName("sessionInfo");
                entity.Property(e => e.Landblock).HasColumnName("landblock");
                entity.Property(e => e.Location).HasColumnName("location");
                entity.Property(e => e.IsLoggingOut).HasColumnName("isLoggingOut");
                entity.Property(e => e.IsInDeathProcess).HasColumnName("isInDeathProcess");
                entity.Property(e => e.FoundOnLandblock).HasColumnName("foundOnLandblock");
                entity.Property(e => e.ForcedLogOffRequested).HasColumnName("forcedLogOffRequested");
                entity.Property(e => e.PkLogoutState).HasColumnName("pkLogoutState");
                entity.Property(e => e.MaterializedLogoutState).HasColumnName("materializedLogoutState");
                entity.Property(e => e.LogoffPath).HasColumnName("logoffPath");
                entity.Property(e => e.CreatedAtUtc).HasColumnName("createdAtUtc");
            });

            modelBuilder.Entity<AllegianceHometownTown>(entity =>
            {
                entity.HasKey(e => e.TownId).HasName("PRIMARY");
                entity.ToTable("allegiance_hometown_town");
                entity.Property(e => e.TownId).HasColumnName("town_id");
                entity.Property(e => e.TownName).HasColumnName("town_name");
                entity.Property(e => e.OwnerMonarchId).HasColumnName("owner_monarch_id");
                entity.Property(e => e.OwnerAllegianceName).HasColumnName("owner_allegiance_name");
                entity.Property(e => e.CapturedAt).HasColumnName("captured_at");
                entity.Property(e => e.ConflictPhase).HasColumnName("conflict_phase");
                entity.Property(e => e.ConflictAttackerMonarchId).HasColumnName("conflict_attacker_monarch_id");
                entity.Property(e => e.ConflictAttackerName).HasColumnName("conflict_attacker_name");
                entity.Property(e => e.ConflictStartTime).HasColumnName("conflict_start_time");
                entity.Property(e => e.Phase2StartTime).HasColumnName("phase2_start_time");
            });

            modelBuilder.Entity<AllegianceHometownEvent>(entity =>
            {
                entity.HasKey(e => e.EventId).HasName("PRIMARY");
                entity.ToTable("allegiance_hometown_event");
                entity.Property(e => e.EventId).HasColumnName("event_id");
                entity.Property(e => e.TownId).HasColumnName("town_id");
                entity.Property(e => e.AttackerMonarchId).HasColumnName("attacker_monarch_id");
                entity.Property(e => e.AttackerAllegianceName).HasColumnName("attacker_allegiance_name");
                entity.Property(e => e.DefenderMonarchId).HasColumnName("defender_monarch_id");
                entity.Property(e => e.DefenderAllegianceName).HasColumnName("defender_allegiance_name");
                entity.Property(e => e.EventStartTime).HasColumnName("event_start_time");
                entity.Property(e => e.Phase2StartTime).HasColumnName("phase2_start_time");
                entity.Property(e => e.EventEndTime).HasColumnName("event_end_time");
                entity.Property(e => e.Outcome).HasColumnName("outcome");
            });

            modelBuilder.Entity<AllegianceHometownBlacklist>(entity =>
            {
                entity.HasKey(e => e.MonarchId).HasName("PRIMARY");
                entity.ToTable("allegiance_hometown_blacklist");
                entity.Property(e => e.MonarchId).HasColumnName("monarch_id");
                entity.Property(e => e.AllegianceName).HasColumnName("allegiance_name");
                entity.Property(e => e.Reason).HasColumnName("reason");
                entity.Property(e => e.AddedBy).HasColumnName("added_by");
                entity.Property(e => e.AddedAt).HasColumnName("added_at");
            });

            modelBuilder.Entity<MovementViolationLog>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("movement_violation_log");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CharacterId).HasColumnName("character_id");
                entity.Property(e => e.CharacterName).HasColumnName("character_name");
                entity.Property(e => e.AccountName).HasColumnName("account_name");
                entity.Property(e => e.ViolationType).HasColumnName("violation_type").HasMaxLength(64);
                entity.Property(e => e.ObservedSpeed).HasColumnName("observed_speed");
                entity.Property(e => e.AllowedSpeed).HasColumnName("allowed_speed");
                entity.Property(e => e.SuspicionScore).HasColumnName("suspicion_score");
                entity.Property(e => e.Location).HasColumnName("location");
                entity.Property(e => e.ViolationDateTime).HasColumnName("violation_datetime");
            });

            modelBuilder.Entity<SeasonCharacterStats>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("season_character_stats");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.CharacterId).HasColumnName("character_id");
                entity.Property(e => e.CharacterName).HasColumnName("character_name");
                entity.Property(e => e.PkKills).HasColumnName("pk_kills");
                entity.Property(e => e.PkDeaths).HasColumnName("pk_deaths");
                entity.Property(e => e.PkKillStreakBest).HasColumnName("pk_kill_streak_best");
                entity.Property(e => e.PkKillStreakCur).HasColumnName("pk_kill_streak_cur");
                entity.Property(e => e.BountiesCompleted).HasColumnName("bounties_completed");
            });

            modelBuilder.Entity<SeasonMilestone>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("season_milestone");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.WeekNumber).HasColumnName("week_number");
                entity.Property(e => e.SnapshotDatetime).HasColumnName("snapshot_datetime");
            });

            modelBuilder.Entity<SeasonMilestoneLeader>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PRIMARY");
                entity.ToTable("season_milestone_leader");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.MilestoneId).HasColumnName("milestone_id");
                entity.Property(e => e.WeekNumber).HasColumnName("week_number");
                entity.Property(e => e.Category).HasColumnName("category");
                entity.Property(e => e.Rank).HasColumnName("rank");
                entity.Property(e => e.CharacterId).HasColumnName("character_id");
                entity.Property(e => e.CharacterName).HasColumnName("character_name");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.RewardClaimed).HasColumnName("reward_claimed");
                entity.Property(e => e.ClaimedDatetime).HasColumnName("claimed_datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
