namespace Minesweeper.WebAPI.Mappers
{
    public static class PlayerStateMapper
    {
        public static Contracts.Responses.Game.PlayerState ToApiContract(GameServices.Contracts.Responses.PlayerState serviceContract)
        {
            return new Contracts.Responses.Game.PlayerState
            {
                PlayerId = serviceContract.PlayerId,
                Points = serviceContract.Points
            };
        }
    }
}