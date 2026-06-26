-- AddAllegianceHometownBlacklist.sql
-- Adds allegiance_hometown_blacklist table.
-- Run ONCE after AddAllegianceHometownFeature.sql has been applied.

USE `classicpvp_log`;

CREATE TABLE IF NOT EXISTS `allegiance_hometown_blacklist` (
  `monarch_id`       INT UNSIGNED  NOT NULL,
  `allegiance_name`  VARCHAR(128)  NOT NULL,
  `reason`           VARCHAR(256)      NULL,
  `added_by`         VARCHAR(64)   NOT NULL,
  `added_at`         DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`monarch_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
