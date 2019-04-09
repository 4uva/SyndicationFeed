using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SyndicationFeed.Common.Models;
using SyndicationFeed.Models;

namespace SyndicationFeed.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly UserManager<IdentityUser> userManager;
        readonly ITokenService tokenService;

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterInfo info)
        {
            // TODO: validate
            var user = new IdentityUser { UserName = info.UserName };
            var createResult = await userManager.CreateAsync(user, info.Password);
            if (!createResult.Succeeded)
            {
                var descriptions = createResult.Errors.Select(error => error.Description).ToList();
                return BadRequest(descriptions);
            }
            else
            {
                //return StatusCode(StatusCodes.Status201Created); // TODO redirect?
                return CreatedAtAction(
                    nameof(Login),
                    new { info = new LoginInfo() { UserName = info.UserName, Password = info.Password } },
                    null); // is null ok here?
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            var user = await userManager.FindByNameAsync(info.UserName);
            if (user == null)
                return Unauthorized(); // no such user

            var isPasswordValid = await userManager.CheckPasswordAsync(user, info.Password);
            if (!isPasswordValid)
                return Unauthorized();

            var token = tokenService.GenerateToken(user);

            return Ok(token);
        }
    }
}
