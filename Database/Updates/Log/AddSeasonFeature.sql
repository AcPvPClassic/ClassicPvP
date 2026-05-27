-- AddSeasonFeature.sql
-- Creates the three tables used by the Season leaderboard system.
-- The "season" is the lifetime of the server (no multi-season infrastructure needed).
-- Run ONCE on any ace_log instance that does not yet have these tables.
-- Safe to re-run: DROP IF EXISTS guards all table creation.

USE `ace_log`;

-- ---------------------------------------------------------------------------
-- season_character_stats
-- Per-character lifetime open-world PK stats and bounty completions.
-- Complements arena_character_stats (which tracks arena-specific stats).
-- Open-world kills are those where killer_arena_player_id IS NULL in pk_kills_log.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `season_character_stats`;
CREATE TABLE `season_character_stats` (
  `id`                    INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `character_id`          INT UNSIGNED  NOT NULL,
  `character_name`        VARCHAR(255),
  -- Open-world PK
  `pk_kills`              INT UNSIGNED  NOT NULL DEFAULT 0,
  `pk_deaths`             INT UNSIGNED  NOT NULL DEFAULT 0,
  `pk_kill_streak_best`   INT UNSIGNED  NOT NULL DEFAULT 0,
  `pk_kill_streak_cur`    INT UNSIGNED  NOT NULL DEFAULT 0,
  -- Bounty system
  `bounties_completed`    INT UNSIGNED  NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`),
  UNIQUE KEY `uq_scs_char` (`character_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE INDEX IF NOT EXISTS idx_scs_pk_kills   ON season_character_stats (pk_kills);
CREATE INDEX IF NOT EXISTS idx_scs_streak     ON season_character_stats (pk_kill_streak_best);
CREATE INDEX IF NOT EXISTS idx_scs_bounties   ON season_character_stats (bounties_completed);

-- ---------------------------------------------------------------------------
-- season_milestone
-- One row per weekly Sunday snapshot.  week_number is a simple incrementing
-- counter maintained in-memory by SeasonManager and persisted here.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `season_milestone`;
CREATE TABLE `season_milestone` (
  `id`                SMALLINT UNSIGNED NOT NULL AUTO_INCREMENT,
  `week_number`       SMALLINT UNSIGNED NOT NULL,
  `snapshot_datetime` DATETIME          NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- season_milestone_leader
-- Top-10 leaders per category at each milestone snapshot.
-- reward_claimed tracks whether the player has collected their reward items.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `season_milestone_leader`;
CREATE TABLE `season_milestone_leader` (
  `id`               INT UNSIGNED      NOT NULL AUTO_INCREMENT,
  `milestone_id`     SMALLINT UNSIGNED NOT NULL,
  `week_number`      SMALLINT UNSIGNED NOT NULL,   -- denormalised for easier queries
  `category`         VARCHAR(32)       NOT NULL,
  `rank`             TINYINT UNSIGNED  NOT NULL,   -- 1–10
  `character_id`     INT UNSIGNED      NOT NULL,
  `character_name`   VARCHAR(255),
  `score`            BIGINT UNSIGNED   NOT NULL,   -- raw score at snapshot time
  `reward_claimed`   BIT               NOT NULL DEFAULT 0,
  `claimed_datetime` DATETIME,
  PRIMARY KEY (`id`),
  INDEX `idx_sml_char_unclaimed` (`character_id`, `reward_claimed`),
  INDEX `idx_sml_milestone`      (`milestone_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
