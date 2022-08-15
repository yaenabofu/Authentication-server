using auth_web_api.Models.DatabaseObjects;
using System;
using System.Threading.Tasks;

namespace auth_web_api.Repositories.RefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByToken(string token);
        Task Create(RefreshToken refreshToken);
        Task Delete(Guid id);
    }
}
