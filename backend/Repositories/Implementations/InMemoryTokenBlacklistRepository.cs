using Microsoft.Extensions.Caching.Memory;

namespace Walks.API.Repositories
{
    public class InMemoryTokenBlacklistRepository : ITokenBlacklistRepository
    {
        private const string RevokedTokenKeyPrefix = "revoked-token:";
        private readonly IMemoryCache _memoryCache;

        public InMemoryTokenBlacklistRepository(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void Revoke(string jti, DateTime expiresAtUtc)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return;
            }

            var absoluteExpiration = expiresAtUtc > DateTime.UtcNow
                ? expiresAtUtc
                : DateTime.UtcNow.AddMinutes(1);

            _memoryCache.Set(GetCacheKey(jti), true, new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            });
        }

        public bool IsRevoked(string jti)
        {
            if (string.IsNullOrWhiteSpace(jti))
            {
                return false;
            }

            return _memoryCache.TryGetValue(GetCacheKey(jti), out _);
        }

        private static string GetCacheKey(string jti) => $"{RevokedTokenKeyPrefix}{jti}";
    }
}

