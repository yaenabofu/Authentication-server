using auth_web_api.Models.DatabaseObjects;
using auth_web_api.Models.Requests;
using auth_web_api.Models.Responses;
using auth_web_api.Repositories.Passwordhashers;
using auth_web_api.Repositories.TokenGenerators;
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
        private readonly AccessTokenGenerator accessTokenGenerator;
        public AuthenticationController(IUserRepository userRepository, IPasswordHasher passwordHasher,
            AccessTokenGenerator accessTokenGenerator)
        {
            this.accessTokenGenerator = accessTokenGenerator;
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
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

            string accessToken = accessTokenGenerator.GenerateToken(userByLogin);

            return Ok(new AuthenticatedUserResponse()
            {
                AccessToken = accessToken
            });
        }

        private IActionResult BadRequestModelState()
        {
            IEnumerable<string> errorMessages = ModelState.Values
                   .SelectMany(value => value.Errors.Select(c => c.ErrorMessage));

            return BadRequest(new ErrorResponse(errorMessages));
        }
    }
}
