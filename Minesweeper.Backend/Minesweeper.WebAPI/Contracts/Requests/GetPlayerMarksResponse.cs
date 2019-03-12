using Minesweeper.GameServices.Contracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class GetPlayerMarksResponse
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MarkType[,] Marks { get; set; }
    }
}