namespace Minesweeper.GameServices.Contracts.Commands
{
    public class NewGameCommand
    {
        public NewGameCommand(string hostPlayerId, string invitedPlayerId, int tableRows, int tableColumns, int mineCount)
        {
            HostPlayerId = hostPlayerId;
            InvitedPlayerId = invitedPlayerId;
            TableRows = tableRows;
            TableColumns = tableColumns;
            MineCount = mineCount;
        }

        public string HostPlayerId { get; }

        public string InvitedPlayerId { get; }

        public int TableRows { get; }

        public int TableColumns { get; }

        public int MineCount { get; }
    }
}