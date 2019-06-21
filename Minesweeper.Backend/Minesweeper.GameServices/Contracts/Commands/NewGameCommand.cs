namespace Minesweeper.GameServices.Contracts.Commands
{
    public class NewGameCommand
    {
        public string HostPlayerId { get; set; }

        public string HostPlayerDisplayName { get; set; }

        public string InvitedPlayerId { get; set; }

        public int TableRows { get; set; }

        public int TableColumns { get; set; }

        public int MineCount { get; set; }
    }
}