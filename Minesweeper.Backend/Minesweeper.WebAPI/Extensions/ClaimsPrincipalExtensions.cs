using System.Security.Claims;
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

        public static UserInfo ToUserInfo(this ClaimsPrincipal claimsPrincipal)
        {
            return new UserInfo
            {
                Id = claimsPrincipal.GetUserId(),
                Email = claimsPrincipal.GetEmail()
            };
        }
    }
}