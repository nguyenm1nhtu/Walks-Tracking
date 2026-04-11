namespace Walks.API.Repositories
{
    public interface ITokenBlacklistRepository
    {
        void Revoke(string jti, DateTime expiresAtUtc);
        bool IsRevoked(string jti);
    }
}

