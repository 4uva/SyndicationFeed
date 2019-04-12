using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SyndicationFeed.Controllers
{
    static class UserHelper
    {
        public static async Task<IdentityUser> GetUserAsync(
            ClaimsPrincipal principal, UserManager<IdentityUser> userManager)
        {
            // userManager.GetUserAsync(User) doesn't work:
            // https://stackoverflow.com/q/51119926/10243782
            var userName = principal.Identity.Name;
            return await userManager.FindByNameAsync(userName);
        }
    }
}
