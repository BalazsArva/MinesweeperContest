namespace Minesweeper.WebAPI.Mappers
{
    public static class GameStatusEnumMapper
    {
        public static Contracts.Responses.Game.GameStatus ToApiContract(GameServices.Contracts.Responses.GameStatus serviceContract)
        {
            return serviceContract == GameServices.Contracts.Responses.GameStatus.Finished
                ? Contracts.Responses.Game.GameStatus.Finished
                : serviceContract == GameServices.Contracts.Responses.GameStatus.InProgress
                    ? Contracts.Responses.Game.GameStatus.InProgress
                    : Contracts.Responses.Game.GameStatus.NotStarted;
        }
    }
}