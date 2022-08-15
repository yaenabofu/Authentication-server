using auth_web_api.Models;
using auth_web_api.Models.DatabaseObjects;

namespace auth_web_api.Repositories.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;

        public RefreshTokenGenerator(AuthenticationConfiguration authenticationConfiguration, TokenGenerator tokenGenerator)
        {
            this._configuration = authenticationConfiguration;
            this._tokenGenerator = tokenGenerator;
        }

        public string GenerateToken()
        {
            string token = _tokenGenerator.GenerateToken(
                _configuration.RefreshTokenSecret,
                _configuration.Issuer,
                _configuration.Audience,
                _configuration.RefreshTokenExpirationMinutes,
                null);

            return token;
        }
    }
}
