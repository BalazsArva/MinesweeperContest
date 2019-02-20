using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        public async Task<NewGameInfo> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task JoinGameAsync(string gameId, string player2Id, string playerDisplayName, string entryToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}