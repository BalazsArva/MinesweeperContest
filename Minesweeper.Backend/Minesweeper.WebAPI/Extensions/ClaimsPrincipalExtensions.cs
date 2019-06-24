using System.Security.Claims;
using Minesweeper.Common;
using Minesweeper.WebAPI.Contracts.Responses.Account;

namespace Minesweeper.WebAPI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(ClaimTypes.Email);
        }

        public static string GetDisplayName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirstValue(CustomClaimTypes.DisplayName);
        }

        public static UserInfo ToUserInfo(this ClaimsPrincipal claimsPrincipal)
        {
            return new UserInfo
            {
                Id = claimsPrincipal.GetUserId(),
                Email = claimsPrincipal.GetEmail(),
                DisplayName = claimsPrincipal.GetDisplayName()
            };
        }
    }
}