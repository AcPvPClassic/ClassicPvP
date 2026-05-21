-- AddStuckCharacterLogFeature.sql
-- Adds the stuck_character_log table to an existing ace_log database.
-- Run ONCE on any instance that has LogBase.sql already applied but predates
-- the stuck-character diagnostic logging feature.

USE `ace_log`;

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
