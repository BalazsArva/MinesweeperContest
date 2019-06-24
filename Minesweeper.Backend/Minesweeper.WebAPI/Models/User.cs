using IdentityModel;
using Minesweeper.Common;
using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Models
{
    public class User
    {
        [JsonProperty(JwtClaimTypes.Subject)]
        public string Id { get; set; }

        public string Email { get; set; }

        [JsonProperty(CustomClaimTypes.DisplayName)]
        public string DisplayName { get; set; }
    }
}