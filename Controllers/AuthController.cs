using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Walks.API.Models.DTOs;
using Walks.API.Repositories;

namespace Walks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenRepository = tokenRepository;
        }
        
        // POST: api/Auth/Register
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            var requestedRoles = registerRequest.Roles?
                .Where(role => !string.IsNullOrWhiteSpace(role))
                .Select(role => role.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray() ?? [];

            if (requestedRoles.Length == 0)
            {
                requestedRoles = ["Reader"];
            }

            var invalidRoles = new List<string>();

            foreach (var role in requestedRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    invalidRoles.Add(role);
                }
            }

            if (invalidRoles.Count > 0)
            {
                ModelState.AddModelError(nameof(registerRequest.Roles), $"Invalid roles: {string.Join(", ", invalidRoles)}");
                return ValidationProblem(ModelState);
            }

            var identityUser = new IdentityUser
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Username
            };

            var result = await _userManager.CreateAsync(identityUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            var addRolesResult = await _userManager.AddToRolesAsync(identityUser, requestedRoles);

            if (!addRolesResult.Succeeded)
            {
                await _userManager.DeleteAsync(identityUser);

                foreach (var error in addRolesResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            return Ok("User registered successfully");
        }

        // POST: api/Auth/Login
        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Username);
            if (user == null)
            {
                return BadRequest("Username or password is incorrect.");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
            {
                return BadRequest("Username or password is incorrect.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = _tokenRepository.CreateJWTToken(user, roles.ToList());

            return Ok(new LoginResponseDto
            {
                JwtToken = jwtToken
            });
        }
    }
}
