-- AddArenaFeature.sql
-- Adds arena tables and pk_kills_log arena columns to an existing ace_log database.
-- Run ONCE on any instance that has LogBase.sql already applied but predates the
-- arena feature.  Safe to re-run: DROP IF EXISTS guards prevent duplicate errors.

USE `ace_log`;

-- Add arena player FK columns to pk_kills_log (idempotent via IF NOT EXISTS emulation)
ALTER TABLE `pk_kills_log`
  ADD COLUMN IF NOT EXISTS `killer_arena_player_id` INT,
  ADD COLUMN IF NOT EXISTS `victim_arena_player_id` INT;

-- ---------------------------------------------------------------------------
-- arena_event
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `arena_event`;
CREATE TABLE `arena_event` (
  `id`                INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `event_type`        VARCHAR(16),
  `location`          INT UNSIGNED,
  `status`            INT,
  `start_datetime`    DATETIME,
  `end_datetime`      DATETIME,
  `winning_team_guid` VARCHAR(36),
  `cancel_reason`     VARCHAR(500),
  `is_overtime`       BIT           NOT NULL DEFAULT (0),
  `create_datetime`   DATETIME,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- arena_player
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `arena_player`;
CREATE TABLE `arena_player` (
  `id`                  INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `character_id`        INT UNSIGNED,
  `character_name`      VARCHAR(255),
  `character_level`     INT UNSIGNED,
  `event_type`          VARCHAR(16),
  `monarch_id`          INT UNSIGNED,
  `monarch_name`        VARCHAR(255),
  `event_id`            INT UNSIGNED,
  `team_guid`           CHAR(36),
  `is_eliminated`       BIT,
  `finish_place`        INT,
  `total_deaths`        INT UNSIGNED,
  `total_kills`         INT UNSIGNED,
  `total_dmg_dealt`     INT UNSIGNED,
  `total_dmg_received`  INT UNSIGNED,
  `create_datetime`     DATETIME,
  `player_ip`           VARCHAR(25),
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- arena_character_stats
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `arena_character_stats`;
CREATE TABLE `arena_character_stats` (
  `id`                  INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `character_id`        INT UNSIGNED,
  `character_name`      VARCHAR(255),
  `event_type`          VARCHAR(12),
  `total_matches`       INT UNSIGNED,
  `total_wins`          INT UNSIGNED,
  `total_losses`        INT UNSIGNED,
  `total_draws`         INT UNSIGNED,
  `total_disqualified`  INT UNSIGNED,
  `total_deaths`        INT UNSIGNED,
  `total_kills`         INT UNSIGNED,
  `total_dmg_dealt`     INT UNSIGNED,
  `total_dmg_received`  INT UNSIGNED,
  `rank_points`         INT UNSIGNED,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Arena indexes (also covered by AddLogDbIndexes.sql; CREATE INDEX IF NOT EXISTS
-- is available in MySQL 8.0+)
CREATE INDEX IF NOT EXISTS idx_arena_character_stats_char       ON arena_character_stats (character_id);
CREATE INDEX IF NOT EXISTS idx_arena_character_stats_event      ON arena_character_stats (event_type);
CREATE INDEX IF NOT EXISTS idx_arena_character_stats_char_event ON arena_character_stats (character_id, event_type);
CREATE INDEX IF NOT EXISTS idx_arena_character_stats_event_rank ON arena_character_stats (event_type, rank_points);
CREATE INDEX IF NOT EXISTS idx_arena_player_event               ON arena_player           (event_id);
