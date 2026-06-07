USE `ace_auth`;

-- Rework account_ip_binding to accumulate all IPs ever used by an account,
-- rather than tracking only the single current IP.
--
-- The global uniqueness of IPs (one account per IP) is preserved via uidx_ip.
-- The per-account uniqueness key is dropped so that each account may accrue
-- as many IP rows as it has ever logged in from.

ALTER TABLE `account_ip_binding`
    DROP INDEX `uidx_account`;
