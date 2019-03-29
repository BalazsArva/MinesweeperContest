using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class NewGameRequest
    {
        [JsonConstructor]
        public NewGameRequest(int tableRows, int tableColumns, int mineCount, string invitedPlayerId)
        {
            TableRows = tableRows;
            TableColumns = tableColumns;
            MineCount = mineCount;
            InvitedPlayerId = invitedPlayerId;
        }

        public int TableRows { get; }

        public int TableColumns { get; }

        public int MineCount { get; }

        public string InvitedPlayerId { get; }
    }
}