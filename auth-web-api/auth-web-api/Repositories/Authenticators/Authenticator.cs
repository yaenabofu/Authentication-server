using auth_web_api.Models.DatabaseObjects;
using auth_web_api.Models.Responses;
using auth_web_api.Repositories.RefreshTokenRepository;
using auth_web_api.Repositories.TokenGenerators;
using System.Threading.Tasks;

namespace auth_web_api.Repositories.Authenticators
{
    public class Authenticator
    {
        private AccessTokenGenerator accessTokenGenerator;
        private RefreshTokenGenerator refreshTokenGenerator;
        private readonly IRefreshTokenRepository refreshTokenRepository;

        public Authenticator(AccessTokenGenerator accessTokenGenerator,
            RefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            this.accessTokenGenerator = accessTokenGenerator;
            this.refreshTokenGenerator = refreshTokenGenerator;
            this.refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthenticatedUserResponse> Authenticate(User user)
        {
            string accessToken = accessTokenGenerator.GenerateToken(user);
            string refreshToken = refreshTokenGenerator.GenerateToken();

            RefreshToken refreshTokenDataObject = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };

            await refreshTokenRepository.Create(refreshTokenDataObject);

            return new AuthenticatedUserResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
