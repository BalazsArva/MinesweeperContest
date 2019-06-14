namespace Minesweeper.WebAPI.Mappers.Game
{
    public static class PlayerStateMapper
    {
        public static Contracts.Responses.Game.PlayerState ToApiContract(GameServices.Contracts.Responses.Game.PlayerState serviceContract)
        {
            return new Contracts.Responses.Game.PlayerState
            {
                PlayerId = serviceContract.PlayerId,
                Points = serviceContract.Points
            };
        }
    }
}