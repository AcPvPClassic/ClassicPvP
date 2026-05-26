-- AddTownControlFeature.sql
-- Adds town_control_town and town_control_event tables to the ace_log database.
-- Run ONCE on any instance that has LogBase.sql already applied.
-- Safe to re-run: DROP IF EXISTS guards prevent duplicate errors.

USE `ace_log`;

-- ---------------------------------------------------------------------------
-- town_control_town
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `town_control_town`;
CREATE TABLE `town_control_town` (
  `town_id`                    SMALLINT UNSIGNED NOT NULL,
  `town_name`                  VARCHAR(64)       NOT NULL,
  `owner_id`                   INT UNSIGNED,
  `is_in_conflict`             BIT               NOT NULL DEFAULT 0,
  `last_conflict_start_time`   DATETIME,
  `conflict_length`            INT UNSIGNED      NOT NULL DEFAULT 3600,
  `conflict_respite_length`    INT UNSIGNED,
  `attacker_awards_individual` SMALLINT UNSIGNED NOT NULL DEFAULT 5,
  `attacker_awards_total`      SMALLINT UNSIGNED NOT NULL DEFAULT 20,
  `defender_awards_individual` SMALLINT UNSIGNED NOT NULL DEFAULT 5,
  `defender_awards_total`      SMALLINT UNSIGNED NOT NULL DEFAULT 20,
  PRIMARY KEY (`town_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Seed town rows (Holtburg=72, Shoushi=91, Yaraq=102)
INSERT IGNORE INTO `town_control_town`
  (`town_id`, `town_name`, `conflict_length`, `conflict_respite_length`,
   `attacker_awards_individual`, `attacker_awards_total`,
   `defender_awards_individual`, `defender_awards_total`)
VALUES
  (72,  'Holtburg', 3600, 86400, 5, 20, 5, 20),
  (91,  'Shoushi',  3600, 86400, 5, 20, 5, 20),
  (102, 'Yaraq',    3600, 86400, 5, 20, 5, 20);

-- ---------------------------------------------------------------------------
-- town_control_event
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `town_control_event`;
CREATE TABLE `town_control_event` (
  `event_id`           INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `town_id`            SMALLINT UNSIGNED NOT NULL,
  `event_start_time`   DATETIME,
  `event_end_time`     DATETIME,
  `attacker_id`        INT UNSIGNED  NOT NULL,
  `attacker_clan_name` VARCHAR(128),
  `defender_id`        INT UNSIGNED,
  `defender_clan_name` VARCHAR(128),
  `is_attack_success`  BIT,
  PRIMARY KEY (`event_id`),
  KEY `idx_tc_event_town`     (`town_id`),
  KEY `idx_tc_event_attacker` (`attacker_id`),
  KEY `idx_tc_event_town_end` (`town_id`, `event_end_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
