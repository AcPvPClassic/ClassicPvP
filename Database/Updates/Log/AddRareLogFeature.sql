-- AddRareLogFeature.sql
-- Adds the rare_log table to an existing ace_log database.
-- Run ONCE on any instance that has LogBase.sql already applied but predates
-- the rare-item logging feature.

USE `ace_log`;

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
