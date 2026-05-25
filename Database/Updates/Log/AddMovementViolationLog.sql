-- AddMovementViolationLog.sql
-- Adds the movement_violation_log table to the ace_log database.
-- Run ONCE on any instance that has LogBase.sql already applied.
-- Safe to re-run: DROP IF EXISTS guards prevent duplicate errors.

USE `ace_log`;

DROP TABLE IF EXISTS `movement_violation_log`;
CREATE TABLE `movement_violation_log` (
  `id`                 INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  `character_id`       INT UNSIGNED  NOT NULL,
  `character_name`     VARCHAR(255)  NOT NULL DEFAULT '',
  `account_name`       VARCHAR(255)  NOT NULL DEFAULT '',
  `observed_speed`     FLOAT         NOT NULL DEFAULT 0,
  `allowed_speed`      FLOAT         NOT NULL DEFAULT 0,
  `suspicion_score`    FLOAT         NOT NULL DEFAULT 0,
  `location`           VARCHAR(512),
  `violation_datetime` DATETIME      NOT NULL,
  PRIMARY KEY (`id`),
  KEY `idx_mvl_character`  (`character_id`),
  KEY `idx_mvl_account`    (`account_name`),
  KEY `idx_mvl_datetime`   (`violation_datetime`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
