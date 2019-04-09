using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SyndicationFeed.Models
{
    // TODO: is location right?
    public interface ITokenService
    {
        string GenerateToken(IdentityUser user);
    }
}
