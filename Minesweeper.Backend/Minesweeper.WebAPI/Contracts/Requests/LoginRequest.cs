using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class LoginRequest
    {
        [JsonConstructor]
        public LoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }

        public string Password { get; }
    }
}