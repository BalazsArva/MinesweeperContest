namespace Minesweeper.WebAPI.Mappers
{
    public static class PlayersEnumMapper
    {
        public static Contracts.Responses.Game.Players ToApiContract(GameServices.Contracts.Responses.Players serviceContract)
        {
            return serviceContract == GameServices.Contracts.Responses.Players.Player1
                ? Contracts.Responses.Game.Players.Player1
                : Contracts.Responses.Game.Players.Player2;
        }
    }
}