using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Contracts.Responses.Account;
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
        public async Task<IActionResult> GetUserInfo()
        {
            var nameClaim = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            var userId = nameClaim.Value;

            // TODO: Return additional user information
            return Ok(new GetUserInfoResponse
            {
                UserInfo =
                {
                    Id = userId
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest loginModel)
        {
            var token = await LoginUser(loginModel.Email, loginModel.Password);

            if (!string.IsNullOrEmpty(token.AccessToken))
            {
                var user = await GetUserInfo(token.AccessToken);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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
        public async Task<IActionResult> Logout()
        {
            await LogoutUser(User.FindFirstValue(AccessTokenClaimKey));
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        private async Task<OidcTokenResponse> LoginUser(string username, string password)
        {
            using (var client = new HttpClient())
            {
                // The Token Endpoint Authentication Method must be set to POST if you
                // want to send the client_secret in the POST body.
                // If Token Endpoint Authentication Method then client_secret must be
                // combined with client_id and provided as a base64 encoded string
                // in a basic authorization header.
                // e.g. Authorization: basic <base64 encoded ("client_id:client_secret")>
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret),
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("scope", "Minesweeper.Apis.Game Minesweeper.Identity openid")
                });

                var uri = $"{IdentityApiBaseUrl}/connect/token";

                var res = await client.PostAsync(uri, formData);

                var json = await res.Content.ReadAsStringAsync();

                var tokenReponse = JsonConvert.DeserializeObject<OidcTokenResponse>(json);

                return tokenReponse;
            }
        }

        private async Task<bool> LogoutUser(string accessToken)
        {
            // TODO: Should not create a http client all the time. A global singleton one will do.
            using (var client = new HttpClient())
            {
                // The Token Endpoint Authentication Method must be set to POST if you
                // want to send the client_secret in the POST body.
                // If Token Endpoint Authentication Method then client_secret must be
                // combined with client_id and provided as a base64 encoded string
                // in a basic authorization header.
                // e.g. Authorization: basic <base64 encoded ("client_id:client_secret")>
                var formData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("token", accessToken),
                    new KeyValuePair<string, string>("token_type_hint", "access_token"),
                    new KeyValuePair<string, string>("client_id", ClientId),
                    new KeyValuePair<string, string>("client_secret", ClientSecret)
                });

                var uri = $"{IdentityApiBaseUrl}/connect/revocation";

                var res = await client.PostAsync(uri, formData);

                return res.IsSuccessStatusCode;
            }
        }

        private async Task<User> GetUserInfo(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var uri = $"{IdentityApiBaseUrl}/connect/userinfo";

                var res = await client.GetAsync(uri);

                var json = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<User>(json);
            }
        }
    }
}