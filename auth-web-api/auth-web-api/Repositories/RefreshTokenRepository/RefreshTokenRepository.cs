using auth_web_api.Models.DatabaseObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace auth_web_api.Repositories.RefreshTokenRepository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DatabaseContext databaseContext;
        public RefreshTokenRepository(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task Create(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();

            await databaseContext.RefreshTokens.AddAsync(refreshToken);
            await databaseContext.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            RefreshToken refreshToken = await databaseContext.RefreshTokens.FirstOrDefaultAsync(c => c.Id == id);

            databaseContext.RefreshTokens.Remove(refreshToken);
            await databaseContext.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetByToken(string token)
        {
            RefreshToken refreshToken = await databaseContext.RefreshTokens
                .FirstOrDefaultAsync(c => c.Token == token);

            return refreshToken;
        }
    }
}
