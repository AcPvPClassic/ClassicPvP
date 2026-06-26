-- AddAllegianceHometownFeature.sql
-- Adds allegiance_hometown_town and allegiance_hometown_event tables.
-- Replaces town_control_town and town_control_event (those tables are dropped below).
-- Run ONCE on any instance that has LogBase.sql already applied.

USE `classicpvp_log`;

-- ---------------------------------------------------------------------------
-- Drop old TownControl tables
-- ---------------------------------------------------------------------------

DROP TABLE IF EXISTS `town_control_event`;
DROP TABLE IF EXISTS `town_control_town`;

-- ---------------------------------------------------------------------------
-- allegiance_hometown_town
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `allegiance_hometown_town` (
  `town_id`                     TINYINT UNSIGNED  NOT NULL,
  `town_name`                   VARCHAR(64)       NOT NULL,
  `owner_monarch_id`            INT UNSIGNED          NULL,
  `owner_allegiance_name`       VARCHAR(128)          NULL,
  `captured_at`                 DATETIME              NULL,
  `conflict_phase`              TINYINT UNSIGNED  NOT NULL DEFAULT 0 COMMENT '0=none 1=phase1 2=phase2',
  `conflict_attacker_monarch_id` INT UNSIGNED         NULL,
  `conflict_attacker_name`      VARCHAR(128)          NULL,
  `conflict_start_time`         DATETIME              NULL,
  `phase2_start_time`           DATETIME              NULL,
  PRIMARY KEY (`town_id`),
  KEY `idx_aht_owner` (`owner_monarch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Seed all 25 capturable towns
INSERT IGNORE INTO `allegiance_hometown_town` (`town_id`, `town_name`) VALUES
  ( 1, 'Al-Arqas'),
  ( 2, 'Al-Jalima'),
  ( 3, 'Arwic'),
  ( 4, 'Baishi'),
  ( 5, 'Cragstone'),
  ( 6, 'Eastham'),
  ( 7, 'Glenden Wood'),
  ( 8, 'Hebian-to'),
  ( 9, 'Holtburg'),
  (10, 'Khayyaban'),
  (11, 'Lin'),
  (12, 'Lytelthorpe'),
  (13, 'Mayoi'),
  (14, 'Nanto'),
  (15, 'Qalaba''r'),
  (16, 'Rithwic'),
  (17, 'Samsur'),
  (18, 'Sawato'),
  (19, 'Shoushi'),
  (20, 'Tou-Tou'),
  (21, 'Tufa'),
  (22, 'Uziz'),
  (23, 'Yanshi'),
  (24, 'Yaraq'),
  (25, 'Zaikhal');

-- ---------------------------------------------------------------------------
-- allegiance_hometown_event  (audit log)
-- ---------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS `allegiance_hometown_event` (
  `event_id`                INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `town_id`                 TINYINT UNSIGNED  NOT NULL,
  `attacker_monarch_id`     INT UNSIGNED  NOT NULL,
  `attacker_allegiance_name` VARCHAR(128)      NULL,
  `defender_monarch_id`     INT UNSIGNED      NULL  COMMENT 'NULL if town was unowned',
  `defender_allegiance_name` VARCHAR(128)      NULL,
  `event_start_time`        DATETIME      NOT NULL,
  `phase2_start_time`       DATETIME          NULL,
  `event_end_time`          DATETIME          NULL,
  `outcome`                 TINYINT           NULL  COMMENT '0=attacker win 1=defender win 2=phase1 timeout',
  PRIMARY KEY (`event_id`),
  KEY `idx_ahe_town`            (`town_id`),
  KEY `idx_ahe_attacker`        (`attacker_monarch_id`),
  KEY `idx_ahe_town_end`        (`town_id`, `event_end_time`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
