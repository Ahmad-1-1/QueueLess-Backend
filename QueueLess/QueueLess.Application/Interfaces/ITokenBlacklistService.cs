using System;

namespace QueueLess.Application.Interfaces
{
    /// <summary>
    /// Manages a blacklist of invalidated JWT tokens.
    /// When a user logs out, their token's JTI is blacklisted until the token naturally expires.
    /// </summary>
    public interface ITokenBlacklistService
    {
        /// <summary>
        /// Adds a token's JTI to the blacklist until its expiry time.
        /// </summary>
        void Blacklist(string jti, DateTime expiry);

        /// <summary>
        /// Returns true if the given JTI has been blacklisted (i.e. the user has logged out).
        /// </summary>
        bool IsBlacklisted(string jti);
    }
}
