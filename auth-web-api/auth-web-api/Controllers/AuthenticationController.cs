using auth_web_api.Models.DatabaseObjects;
using auth_web_api.Models.Requests;
using auth_web_api.Models.Responses;
using auth_web_api.Repositories.Passwordhashers;
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
        public AuthenticationController(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<string> errorMessages = ModelState.Values
                    .SelectMany(value => value.Errors.Select(c => c.ErrorMessage));

                return BadRequest(new ErrorResponse(errorMessages));
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
    }
}
