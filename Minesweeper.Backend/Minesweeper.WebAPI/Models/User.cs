using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Models
{
    public class User
    {
        [JsonProperty("sub")]
        public string Id { get; set; }
    }
}