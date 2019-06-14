namespace Minesweeper.WebAPI.Mappers.Game
{
    public static class PlayersEnumMapper
    {
        public static Contracts.Responses.Game.Players ToApiContract(GameServices.Contracts.Responses.Game.Players serviceContract)
        {
            return serviceContract == GameServices.Contracts.Responses.Game.Players.Player1
                ? Contracts.Responses.Game.Players.Player1
                : Contracts.Responses.Game.Players.Player2;
        }
    }
}