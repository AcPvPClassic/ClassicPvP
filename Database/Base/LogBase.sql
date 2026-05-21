-- ACE Log Database - Base Schema
-- Creates the ace_log database and all tables from scratch.
-- Run this once on a fresh install.  For incremental upgrades use the
-- scripts under Database/Updates/Log/.

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Create / select database
--

CREATE DATABASE IF NOT EXISTS `ace_log`
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_general_ci;

USE `ace_log`;

-- ---------------------------------------------------------------------------
-- tinker_log
-- Logs every tinkering attempt with outcome and item/salvage details.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `tinker_log`;
CREATE TABLE `tinker_log` (
  `tinkerLogId`             INT UNSIGNED     NOT NULL AUTO_INCREMENT,
  `characterId`             INT UNSIGNED     NOT NULL,
  `characterName`           VARCHAR(50)      NOT NULL,
  `itemBiotaId`             INT UNSIGNED,
  `tinkDateTime`            DATETIME,
  `successChance`           FLOAT,
  `roll`                    FLOAT,
  `isSuccess`               BIT,
  `itemNumPreviousTinks`    INT,
  `itemWorkmanship`         INT,
  `salvageType`             VARCHAR(50),
  `salvageWorkmanship`      INT,
  PRIMARY KEY (`tinkerLogId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- account_session_log
-- One row per account login/logout session.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `account_session_log`;
CREATE TABLE `account_session_log` (
  `sessionLogId`    INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `accountId`       INT UNSIGNED  NOT NULL,
  `accountName`     VARCHAR(50)   NOT NULL,
  `sessionIP`       VARCHAR(45),
  `loginDateTime`   DATETIME,
  `logoutDateTime`  DATETIME,
  PRIMARY KEY (`sessionLogId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- character_login_log
-- One row per character login/logout.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `character_login_log`;
CREATE TABLE `character_login_log` (
  `characterLoginLogId` INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `accountId`           INT UNSIGNED  NOT NULL,
  `accountName`         VARCHAR(50)   NOT NULL,
  `characterId`         INT UNSIGNED  NOT NULL,
  `characterName`       VARCHAR(50)   NOT NULL,
  `sessionIP`           VARCHAR(45),
  `loginDateTime`       DATETIME,
  `logoutDateTime`      DATETIME,
  PRIMARY KEY (`characterLoginLogId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- pk_kills_log
-- Every PK kill, optionally linked to arena players.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `pk_kills_log`;
CREATE TABLE `pk_kills_log` (
  `id`                      INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `killer_id`               INT UNSIGNED  NOT NULL,
  `victim_id`               INT UNSIGNED  NOT NULL,
  `killer_monarch_id`       INT UNSIGNED,
  `victim_monarch_id`       INT UNSIGNED,
  `kill_datetime`           DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `killer_arena_player_id`  INT,
  `victim_arena_player_id`  INT,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- arena_event
-- One row per arena match lifecycle.
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
-- One row per player per arena event (queued + active).
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
-- Aggregate lifetime stats per character per event type; holds ELO rank_points.
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

-- ---------------------------------------------------------------------------
-- rare_log
-- Logs when a player receives a rare item.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `rare_log`;
CREATE TABLE `rare_log` (
  `id`              INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `characterName`   VARCHAR(255),
  `characterId`     INT UNSIGNED,
  `itemName`        VARCHAR(255),
  `itemBiotaId`     INT UNSIGNED,
  `itemWeenieId`    INT UNSIGNED,
  `createDateTime`  DATETIME,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- stuck_character_log
-- Diagnostic log written when the server force-logs a stuck character.
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `stuck_character_log`;
CREATE TABLE `stuck_character_log` (
  `stuckCharacterLogId`     BIGINT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `playerGuid`              INT UNSIGNED,
  `playerName`              VARCHAR(255),
  `accountName`             VARCHAR(255),
  `accountId`               INT UNSIGNED,
  `sessionInfo`             VARCHAR(255),
  `landblock`               VARCHAR(50),
  `location`                VARCHAR(255),
  `isLoggingOut`            BIT,
  `isInDeathProcess`        BIT,
  `foundOnLandblock`        BIT,
  `forcedLogOffRequested`   BIT,
  `pkLogoutState`           INT UNSIGNED,
  `materializedLogoutState` INT UNSIGNED,
  `logoffPath`              VARCHAR(500),
  `createdAtUtc`            DATETIME,
  PRIMARY KEY (`stuckCharacterLogId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------------
-- Indexes  (same set as AddLogDbIndexes.sql)
-- ---------------------------------------------------------------------------

CREATE INDEX idx_account_session_log_accountId         ON account_session_log    (accountId);
CREATE INDEX idx_character_login_log_characterId       ON character_login_log    (characterId);

CREATE INDEX idx_arena_character_stats_char            ON arena_character_stats  (character_id);
CREATE INDEX idx_arena_character_stats_event           ON arena_character_stats  (event_type);
CREATE INDEX idx_arena_character_stats_char_event      ON arena_character_stats  (character_id, event_type);
CREATE INDEX idx_arena_character_stats_event_rank      ON arena_character_stats  (event_type, rank_points);

CREATE INDEX idx_arena_player_event                    ON arena_player           (event_id);

/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
