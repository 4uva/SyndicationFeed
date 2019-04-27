using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SyndicationFeed.Common.Models;
using SyndicationFeed.Models;

namespace SyndicationFeed.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController(
            UserManager<IdentityUser> userManager,
            ITokenService tokenService,
            ILogger<AuthController> logger)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserInfo info)
        {
            logger.LogInformation("Registering new user {user}", info.UserName);
            var user = new IdentityUser { UserName = info.UserName };
            var createResult = await userManager.CreateAsync(user, info.Password);
            if (!createResult.Succeeded)
            {
                var descriptions = createResult.Errors.Select(error => error.Description).ToList();
                logger.LogWarning("Registering user {user} failed: {errors}", info.UserName, descriptions);
                return Conflict(descriptions);
            }
            else
            {
                logger.LogInformation("Registraton succeeded, user {user}", info.UserName);
                return StatusCode(StatusCodes.Status201Created); // TODO redirect?
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Unregister()
        {
            logger.LogInformation("Unregistering current user {user}", User?.Identity?.Name);
            var user = await UserHelper.GetUserAsync(User, userManager);
            if (user == null)
            {
                // deleted elsewhere in the meanwhile?
                // is here a way to distinguish?
                logger.LogWarning("Couldn't find user {user} in the database, unregistration failed", User?.Identity?.Name);
                return NoContent();
            }

            logger.LogInformation("Deleting user {user} from database", user.UserName);
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                var descriptions = deleteResult.Errors.Select(error => error.Description).ToList();
                logger.LogError("Couldn't delete user {user} from the database, errors: {errors}", user?.UserName, descriptions);
                throw new InvalidOperationException(descriptions.FirstOrDefault() ?? "unknown error");
            }
            else
            {
                logger.LogInformation("Deleting user {user} succeeded", user.UserName);
                return NoContent();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            logger.LogInformation("User {user} logging in", info.UserName);
            var user = await userManager.FindByNameAsync(info.UserName);
            if (user == null)
            {
                logger.LogWarning("User {user} not found, login failed", info.UserName);
                return Unauthorized(); // no such user
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, info.Password);
            if (!isPasswordValid)
            {
                logger.LogWarning("User {user} supplied wrong password, login failed", info.UserName);
                return Unauthorized();
            }

            var token = tokenService.GenerateToken(user);

            logger.LogInformation("User {user} login succeeded", info.UserName);
            return Ok(token);
        }

        readonly UserManager<IdentityUser> userManager;
        readonly ITokenService tokenService;
        readonly ILogger logger;
    }
}
