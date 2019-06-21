using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Contracts.Responses.Account;
using Minesweeper.WebAPI.Extensions;
using Minesweeper.WebAPI.Models;
using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Controllers
{
    // See https://github.com/onelogin/openid-connect-dotnet-core-sample
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private const string AccessTokenClaimKey = "access_token";

        // TODO: Move to config
        private const string ClientId = "Minesweeper.API";

        private const string ClientSecret = "your-onelogin-openid-connect-client-secret";
        private const string IdentityApiBaseUrl = "https://localhost:5003";

        // TODO: This returns 500 because of challenge failure instead of 401 when the user is not authenticated. Fix it here and everywhere else as unauthenticated requests will be 500 instead of 401.
        [HttpGet]
        [Authorize]
        public IActionResult GetUserInfo()
        {
            return Ok(new GetUserInfoResponse
            {
                UserInfo = User.ToUserInfo()
            });
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest loginModel, CancellationToken cancellationToken)
        {
            var token = await LoginUserAsync(loginModel.Email, loginModel.Password, cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(token.AccessToken))
            {
                var user = await GetUserInfoAsync(token.AccessToken, cancellationToken).ConfigureAwait(false);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email.ToString()),
                    new Claim(AccessTokenClaimKey, token.AccessToken)
                };

                var userIdentity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(userIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok();
            }

            // TODO: Improve error feedback
            ModelState.AddModelError(string.Empty, "Login Failed");

            return BadRequest(loginModel);
        }

        [HttpDelete]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            await LogoutUserAsync(User.FindFirstValue(AccessTokenClaimKey), cancellationToken);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        private async Task<OidcTokenResponse> LoginUserAsync(string username, string password, CancellationToken cancellationToken)
        {
            // TODO: Reuse a client
            using (var client = new HttpClient())
            {
                var uri = $"{IdentityApiBaseUrl}/connect/token";

                var request = new PasswordTokenRequest
                {
                    Address = uri,
                    Password = password,
                    UserName = username,
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    Scope = "Minesweeper.Apis.Game openid email profile"
                };

                var response = await client.RequestPasswordTokenAsync(request, cancellationToken).ConfigureAwait(false);

                return new OidcTokenResponse
                {
                    AccessToken = response.AccessToken,
                    ExpiresIn = response.ExpiresIn.ToString(CultureInfo.InvariantCulture),
                    RefeshToken = response.RefreshToken,
                    TokenType = response.TokenType
                };
            }
        }

        private async Task<bool> LogoutUserAsync(string accessToken, CancellationToken cancellationToken)
        {
            // TODO: Should not create a http client all the time.
            using (var client = new HttpClient())
            {
                var uri = $"{IdentityApiBaseUrl}/connect/revocation";

                var request = new TokenRevocationRequest
                {
                    Address = uri,
                    ClientId = ClientId,
                    ClientSecret = ClientSecret,
                    Token = accessToken,
                    TokenTypeHint = "access_token"
                };

                var response = await client.RevokeTokenAsync(request, cancellationToken).ConfigureAwait(false);

                // TODO: Improve this
                return !response.IsError;
            }
        }

        private async Task<User> GetUserInfoAsync(string accessToken, CancellationToken cancellationToken)
        {
            using (var client = new HttpClient())
            {
                var uri = $"{IdentityApiBaseUrl}/connect/userinfo";
                var request = new UserInfoRequest
                {
                    Address = uri,
                    Token = accessToken
                };

                var userInfo = await client.GetUserInfoAsync(request, cancellationToken).ConfigureAwait(false);

                return JsonConvert.DeserializeObject<User>(userInfo.Raw);
            }
        }
    }
}