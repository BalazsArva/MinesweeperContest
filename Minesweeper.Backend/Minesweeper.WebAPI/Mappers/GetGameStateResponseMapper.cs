namespace Minesweeper.WebAPI.Mappers
{
    public static class GetGameStateResponseMapper
    {
        public static Contracts.Responses.Game.GetGameStateResponse ToApiResponse(GameServices.Contracts.Responses.GetGameStateResponse serviceResponse)
        {
            return new Contracts.Responses.Game.GetGameStateResponse
            {
                NextPlayer = PlayersEnumMapper.ToApiContract(serviceResponse.NextPlayer),
                RemainingMines = serviceResponse.RemainingMines,
                UtcDateTimeStarted = serviceResponse.UtcDateTimeStarted,
                Status = GameStatusEnumMapper.ToApiContract(serviceResponse.Status),
                Winner = serviceResponse.Winner != null
                    ? PlayersEnumMapper.ToApiContract(serviceResponse.Winner.Value)
                    : (Contracts.Responses.Game.Players?)null
            };
        }
    }
}