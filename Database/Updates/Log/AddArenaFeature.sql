-- AddArenaFeature.sql
-- Adds arena tables and pk_kills_log arena columns to an existing ace_log database.
-- Run ONCE on any instance that has LogBase.sql already applied but predates the
-- arena feature.
--
-- MySQL 8.0 compatible — no MariaDB-specific syntax.
-- ADD COLUMN IF NOT EXISTS is NOT supported by MySQL 8.0; idempotency for column
-- additions is achieved by querying information_schema.COLUMNS before each ALTER.
-- Indexes inside CREATE TABLE are always safe because the tables are dropped first.

USE `ace_log`;

-- ---------------------------------------------------------------------------
-- Add arena player FK columns to pk_kills_log (idempotent via info_schema check)
-- ---------------------------------------------------------------------------

SELECT COUNT(*) INTO @x FROM information_schema.COLUMNS
  WHERE table_schema = DATABASE() AND table_name = 'pk_kills_log'
    AND column_name = 'killer_arena_player_id';
SET @s = IF(@x = 0,
  'ALTER TABLE `pk_kills_log` ADD COLUMN `killer_arena_player_id` INT',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.COLUMNS
  WHERE table_schema = DATABASE() AND table_name = 'pk_kills_log'
    AND column_name = 'victim_arena_player_id';
SET @s = IF(@x = 0,
  'ALTER TABLE `pk_kills_log` ADD COLUMN `victim_arena_player_id` INT',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

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
  `is_overtime`       BIT           NOT NULL DEFAULT 0,
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

-- Arena indexes — declared after CREATE TABLE via information_schema guard
-- (CREATE INDEX IF NOT EXISTS is NOT supported by MySQL 8.0).

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_char';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_char ON arena_character_stats (character_id)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_event';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_event ON arena_character_stats (event_type)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_char_event';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_char_event ON arena_character_stats (character_id, event_type)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_character_stats'
    AND index_name = 'idx_arena_character_stats_event_rank';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_character_stats_event_rank ON arena_character_stats (event_type, rank_points)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;

SELECT COUNT(*) INTO @x FROM information_schema.STATISTICS
  WHERE table_schema = DATABASE() AND table_name = 'arena_player'
    AND index_name = 'idx_arena_player_event';
SET @s = IF(@x = 0,
  'CREATE INDEX idx_arena_player_event ON arena_player (event_id)',
  'SELECT 1');
PREPARE _stmt FROM @s; EXECUTE _stmt; DEALLOCATE PREPARE _stmt;
