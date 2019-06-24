using System.Threading.Tasks;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Minesweeper.Identity.Data.Entities;

namespace Minesweeper.Identity.IdentityServices
{
    public class CustomProfileService : ProfileService<AppUser>
    {
        public CustomProfileService(UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> claimsFactory)
            : base(userManager, claimsFactory)
        {
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context).ConfigureAwait(false);
        }
    }
}