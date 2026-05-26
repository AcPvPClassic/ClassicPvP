-- AddArenaRankingColumns.sql
-- Upgrades arena_character_stats with ELO, decay, and survival tracking.
-- Adds arena_team_stats table for 2v2 team-pair rankings.
-- Run once on any instance that has AddArenaFeature.sql already applied.
--
-- NOTE: MySQL does not support ADD COLUMN IF NOT EXISTS or CREATE INDEX IF NOT EXISTS
-- (those are MariaDB extensions).  This script uses INFORMATION_SCHEMA checks inside
-- a temporary stored procedure to achieve idempotency on plain MySQL 5.7 / 8.x.

USE `ace_log`;

-- ── Idempotent column additions on arena_character_stats ─────────────────────
DROP PROCEDURE IF EXISTS _AddArenaRankingColumns;

DELIMITER $$
CREATE PROCEDURE _AddArenaRankingColumns()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_character_stats'
          AND COLUMN_NAME  = 'elo'
    ) THEN
        ALTER TABLE `arena_character_stats`
            ADD COLUMN `elo` INT UNSIGNED NOT NULL DEFAULT 1500;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_character_stats'
          AND COLUMN_NAME  = 'last_match_datetime'
    ) THEN
        ALTER TABLE `arena_character_stats`
            ADD COLUMN `last_match_datetime` DATETIME;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_character_stats'
          AND COLUMN_NAME  = 'last_decay_datetime'
    ) THEN
        ALTER TABLE `arena_character_stats`
            ADD COLUMN `last_decay_datetime` DATETIME;
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_character_stats'
          AND COLUMN_NAME  = 'total_survived'
    ) THEN
        ALTER TABLE `arena_character_stats`
            ADD COLUMN `total_survived` INT UNSIGNED NOT NULL DEFAULT 0;
    END IF;
END$$
DELIMITER ;

CALL _AddArenaRankingColumns();
DROP PROCEDURE IF EXISTS _AddArenaRankingColumns;

-- Migrate existing ELO data: rank_points held raw ELO for 1v1/2v2 rows.
UPDATE `arena_character_stats`
   SET `elo` = `rank_points`
 WHERE `event_type` IN ('1v1', '2v2')
   AND `rank_points` > 0
   AND `elo` = 1500;

-- ── 2v2 team-pair rankings table ──────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `arena_team_stats` (
  `id`                   INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `team_key`             VARCHAR(40)   NOT NULL,
  `character_id_a`       INT UNSIGNED  NOT NULL,
  `character_name_a`     VARCHAR(255),
  `character_id_b`       INT UNSIGNED  NOT NULL,
  `character_name_b`     VARCHAR(255),
  `elo`                  INT UNSIGNED  NOT NULL DEFAULT 1500,
  `rank_points`          INT UNSIGNED  NOT NULL DEFAULT 0,
  `total_matches`        INT UNSIGNED  NOT NULL DEFAULT 0,
  `total_wins`           INT UNSIGNED  NOT NULL DEFAULT 0,
  `total_losses`         INT UNSIGNED  NOT NULL DEFAULT 0,
  `total_draws`          INT UNSIGNED  NOT NULL DEFAULT 0,
  `total_disqualified`   INT UNSIGNED  NOT NULL DEFAULT 0,
  `total_survived`       INT UNSIGNED  NOT NULL DEFAULT 0,
  `last_match_datetime`  DATETIME,
  `last_decay_datetime`  DATETIME,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_team_key` (`team_key`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ── Idempotent index creation on arena_team_stats ────────────────────────────
DROP PROCEDURE IF EXISTS _AddArenaTeamStatsIndexes;

DELIMITER $$
CREATE PROCEDURE _AddArenaTeamStatsIndexes()
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_team_stats'
          AND INDEX_NAME   = 'idx_arena_team_stats_key'
    ) THEN
        CREATE INDEX idx_arena_team_stats_key    ON `arena_team_stats` (`team_key`);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_team_stats'
          AND INDEX_NAME   = 'idx_arena_team_stats_rank'
    ) THEN
        CREATE INDEX idx_arena_team_stats_rank   ON `arena_team_stats` (`rank_points`);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_team_stats'
          AND INDEX_NAME   = 'idx_arena_team_stats_char_a'
    ) THEN
        CREATE INDEX idx_arena_team_stats_char_a ON `arena_team_stats` (`character_id_a`);
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME   = 'arena_team_stats'
          AND INDEX_NAME   = 'idx_arena_team_stats_char_b'
    ) THEN
        CREATE INDEX idx_arena_team_stats_char_b ON `arena_team_stats` (`character_id_b`);
    END IF;
END$$
DELIMITER ;

CALL _AddArenaTeamStatsIndexes();
DROP PROCEDURE IF EXISTS _AddArenaTeamStatsIndexes;
