using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Models;
using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Controllers
{
    // See https://github.com/onelogin/openid-connect-dotnet-core-sample
    public class AccountController : Controller
    {
        // TODO: Move to config
        private const string ONELOGIN_OPENID_CONNECT_CLIENT_ID = "your-onelogin-openid-connect-client-id";
        private const string ONELOGIN_OPENID_CONNECT_CLIENT_SECRET = "your-onelogin-openid-connect-client-secret";
        private const string ONELOGIN_OIDC_REGION = "openid-connect"; // For EU us openid-connect-eu

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest loginModel, string returnUrl = "/home/dashboard")
        {
            ViewData["ReturnUrl"] = returnUrl;

            var token = await LoginUser(loginModel.Username, loginModel.Password);

            if (!string.IsNullOrEmpty(token.AccessToken))
            {
                // We need to call OIDC again to get the user claims
                var user = await GetUserInfo(token.AccessToken);

                var claims = new List<Claim>
          {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("onelogin-access-token", token.AccessToken)
          };

                var userIdentity = new ClaimsIdentity(claims, "login");

                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                await HttpContext.SignInAsync(principal);

                //Just redirect to our index after logging in.
                return Redirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Login Failed");
            return View(loginModel);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await LogoutUser(User.FindFirstValue("onelogin-access-token"));
            await HttpContext.SignOutAsync();

            // TODO: Provide valid route
            return RedirectToAction("Index", "Home");
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
              new KeyValuePair<string, string>("client_id", ONELOGIN_OPENID_CONNECT_CLIENT_ID),
              new KeyValuePair<string, string>("client_secret", ONELOGIN_OPENID_CONNECT_CLIENT_SECRET),
              new KeyValuePair<string, string>("grant_type", "password"),
              new KeyValuePair<string, string>("scope", "openid profile email")
          });

                var uri = string.Format("https://{0}.onelogin.com/oidc/token", ONELOGIN_OIDC_REGION);

                var res = await client.PostAsync(uri, formData);

                var json = await res.Content.ReadAsStringAsync();

                var tokenReponse = JsonConvert.DeserializeObject<OidcTokenResponse>(json);

                return tokenReponse;
            }
        }

        private async Task<bool> LogoutUser(string accessToken)
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
              new KeyValuePair<string, string>("token", accessToken),
              new KeyValuePair<string, string>("token_type_hint", "access_token"),
              new KeyValuePair<string, string>("client_id", ONELOGIN_OPENID_CONNECT_CLIENT_ID),
              new KeyValuePair<string, string>("client_secret", ONELOGIN_OPENID_CONNECT_CLIENT_SECRET)
          });

                var uri = string.Format("https://{0}.onelogin.com/oidc/token/revocation", ONELOGIN_OIDC_REGION);

                var res = await client.PostAsync(uri, formData);

                return res.IsSuccessStatusCode;
            }
        }

        private async Task<User> GetUserInfo(string accessToken)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var uri = string.Format("https://{0}.onelogin.com/oidc/me", ONELOGIN_OIDC_REGION);

                var res = await client.GetAsync(uri);

                var json = await res.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<User>(json);
            }
        }
    }
}