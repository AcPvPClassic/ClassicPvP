USE `ace_auth`;

-- Table: account_ip_binding
-- Tracks the single IP address permanently associated with each account.
-- One IP may only be bound to one account globally.

CREATE TABLE IF NOT EXISTS `account_ip_binding` (
    `id`          INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    `account_id`  INT UNSIGNED    NOT NULL,
    `ip_address`  VARCHAR(45)     NOT NULL,
    `bound_at`    DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `bound_by`    VARCHAR(10)     NOT NULL DEFAULT 'login',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uidx_ip`      (`ip_address`),
    UNIQUE KEY `uidx_account` (`account_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- Table: account_ip_change_log
-- Audit log of every IP change per account.
-- auto_banned = 1 means this change triggered an automatic ban (2nd change in a calendar month).
-- admin_cleared = 1 means an admin reset this account's binding; the entry is preserved for the
--                    audit trail but does not count toward the monthly change limit.

CREATE TABLE IF NOT EXISTS `account_ip_change_log` (
    `id`            INT UNSIGNED    NOT NULL AUTO_INCREMENT,
    `account_id`    INT UNSIGNED    NOT NULL,
    `old_ip`        VARCHAR(45)     NOT NULL,
    `new_ip`        VARCHAR(45)     NOT NULL,
    `changed_at`    DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `auto_banned`   TINYINT(1)      NOT NULL DEFAULT 0,
    `admin_cleared` TINYINT(1)      NOT NULL DEFAULT 0,
    PRIMARY KEY (`id`),
    KEY `idx_account_date` (`account_id`, `changed_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
