using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Raven.Client.Documents;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IDocumentStore _documentStore;

        public GameService(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var isPlayer1 = game.Player1.PlayerId == playerId;
                var isPlayer2 = game.Player2.PlayerId == playerId;

                if (!isPlayer1 && !isPlayer2)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                var playerMarks = isPlayer1 ? game.Player1Marks : game.Player2Marks;

                return EnumArrayCloner.CloneAndMap(playerMarks, MarkTypeConverter.ToContract);
            }
        }

        public async Task<VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Validate user id
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                return EnumArrayCloner.CloneAndMap(game.VisibleTable, FieldTypeConverter.ToContract);
            }
        }
    }
}