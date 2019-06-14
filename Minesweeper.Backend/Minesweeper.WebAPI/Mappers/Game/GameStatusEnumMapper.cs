namespace Minesweeper.WebAPI.Mappers.Game
{
    public static class GameStatusEnumMapper
    {
        public static Contracts.Responses.Game.GameStatus ToApiContract(GameServices.Contracts.Responses.Game.GameStatus serviceContract)
        {
            return serviceContract == GameServices.Contracts.Responses.Game.GameStatus.Finished
                ? Contracts.Responses.Game.GameStatus.Finished
                : serviceContract == GameServices.Contracts.Responses.Game.GameStatus.InProgress
                    ? Contracts.Responses.Game.GameStatus.InProgress
                    : Contracts.Responses.Game.GameStatus.NotStarted;
        }
    }
}