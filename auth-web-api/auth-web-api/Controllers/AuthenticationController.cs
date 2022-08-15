using auth_web_api.Models.DatabaseObjects;
using auth_web_api.Models.Requests;
using auth_web_api.Models.Responses;
using auth_web_api.Repositories.Authenticators;
using auth_web_api.Repositories.Passwordhashers;
using auth_web_api.Repositories.RefreshTokenRepository;
using auth_web_api.Repositories.TokenGenerators;
using auth_web_api.Repositories.TokenValidators;
using auth_web_api.Repositories.UserRepository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace auth_web_api.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly RefreshTokenValidator refreshTokenValidator;
        private readonly IRefreshTokenRepository refreshTokenRepository;
        private readonly Authenticator authenticator;

        public AuthenticationController(IUserRepository userRepository, IPasswordHasher passwordHasher,
            RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository, Authenticator authenticator)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.refreshTokenValidator = refreshTokenValidator;
            this.refreshTokenRepository = refreshTokenRepository;
            this.authenticator = authenticator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                BadRequestModelState();
            }

            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("Password does not match confirmed password"));
            }

            User userByLogin = await userRepository.GetByLogin(registerRequest.Login);

            if (userByLogin != null)
            {
                return Conflict(new ErrorResponse("Login already exists"));
            }

            User userByEmail = await userRepository.GetByEmail(registerRequest.Email);

            if (userByEmail != null)
            {
                return Conflict(new ErrorResponse("Email already exists"));
            }

            string hashedPassword = passwordHasher.Hash(registerRequest.Password);

            User userToRegister = new User()
            {
                Email = registerRequest.Email,
                HashedPassword = hashedPassword,
                Login = registerRequest.Login
            };

            await userRepository.Create(userToRegister);

            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestModelState();
            }

            bool isValidRefreshToken = refreshTokenValidator.Validate(refreshRequest.RefreshToken);

            if (!isValidRefreshToken)
            {
                return BadRequest(new ErrorResponse("Refresh token is invalid"));
            }

            RefreshToken refreshToken = await refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshToken == null)
            {
                return NotFound(new ErrorResponse("Invalid refresh token"));
            }

            await refreshTokenRepository.Delete(refreshToken.Id);

            User user = await userRepository.GetById(refreshToken.UserId);

            if (user == null)
            {
                return NotFound(new ErrorResponse("User not found"));
            }

            AuthenticatedUserResponse authenticatedUserResponse = await authenticator.Authenticate(user);

            return Ok(authenticatedUserResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                BadRequestModelState();
            }

            User userByLogin = await userRepository.GetByLogin(loginRequest.Login);

            if (userByLogin == null)
            {
                return Unauthorized();
            }

            bool isGivenPasswordMatchActualPasswordHash = passwordHasher.
                VerifyPassword(loginRequest.Password, userByLogin.HashedPassword);

            if (!isGivenPasswordMatchActualPasswordHash)
            {
                return Unauthorized();
            }

            AuthenticatedUserResponse authenticatedUserResponse = await authenticator.Authenticate(userByLogin);

            return Ok(authenticatedUserResponse);
        }

        private IActionResult BadRequestModelState()
        {
            IEnumerable<string> errorMessages = ModelState.Values
                   .SelectMany(value => value.Errors.Select(c => c.ErrorMessage));

            return BadRequest(new ErrorResponse(errorMessages));
        }
    }
}
