using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Minesweeper.Identity.Data.Entities;

namespace Minesweeper.Identity.IdentityServices
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser>
    {
        public CustomClaimsPrincipalFactory(UserManager<AppUser> userManager, IOptions<IdentityOptions> identityOptionsAccessor)
            : base(userManager, identityOptionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(AppUser user)
        {
            var result = await base.CreateAsync(user).ConfigureAwait(false);

            return result;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var result = await base.GenerateClaimsAsync(user).ConfigureAwait(false);

            return result;
        }
    }
}