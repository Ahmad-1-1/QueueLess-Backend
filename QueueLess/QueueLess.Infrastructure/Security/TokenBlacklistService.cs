using System;
using System.Collections.Concurrent;
using QueueLess.Application.Interfaces;

namespace QueueLess.Infrastructure.Security
{
    /// <summary>
    /// In-memory JWT token blacklist.
    /// Stores invalidated token JTIs (e.g. after logout) until they naturally expire.
    /// Registered as a Singleton so the blacklist persists across requests.
    /// Note: Resets on application restart. For production persistence, swap to Redis or DB.
    /// </summary>
    public class TokenBlacklistService : ITokenBlacklistService
    {
        // Maps JTI -> token expiry time (UTC)
        private readonly ConcurrentDictionary<string, DateTime> _blacklist = new();

        public void Blacklist(string jti, DateTime expiry)
        {
            _blacklist[jti] = expiry;
            PruneExpired();
        }

        public bool IsBlacklisted(string jti)
        {
            if (!_blacklist.TryGetValue(jti, out var expiry))
                return false;

            // If the token has naturally expired, remove it and treat as not blacklisted
            // (the JWT middleware will reject it anyway due to expiry)
            if (DateTime.UtcNow >= expiry)
            {
                _blacklist.TryRemove(jti, out _);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes entries whose tokens have already expired to keep memory usage low.
        /// Called on every Blacklist() operation.
        /// </summary>
        private void PruneExpired()
        {
            var now = DateTime.UtcNow;
            foreach (var key in _blacklist.Keys)
            {
                if (_blacklist.TryGetValue(key, out var expiry) && now >= expiry)
                    _blacklist.TryRemove(key, out _);
            }
        }
    }
}
