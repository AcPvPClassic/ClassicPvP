using System;

namespace ACE.Database.Models.Auth;

public partial class AccountIpBinding
{
    public uint Id { get; set; }

    public uint AccountId { get; set; }

    /// <summary>
    /// The IP address (IPv4 or IPv6) bound to this account. Globally unique — one IP per account, worldwide.
    /// </summary>
    public string IpAddress { get; set; }

    public DateTime BoundAt { get; set; }

    /// <summary>
    /// "login" when bound automatically on first login; "admin" when set by an admin command.
    /// </summary>
    public string BoundBy { get; set; }
}
