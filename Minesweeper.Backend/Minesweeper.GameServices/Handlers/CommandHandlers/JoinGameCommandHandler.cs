using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Raven.Client.Documents;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class JoinGameCommandHandler : IJoinGameCommandHandler
    {
        private readonly IDocumentStore _documentStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public JoinGameCommandHandler(IDocumentStore documentStore, IDateTimeProvider dateTimeProvider)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task HandleAsync(JoinGameCommand command, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var player2Id = command.PlayerId;

                var game = await session.LoadGameAsync(command.GameId, cancellationToken).ConfigureAwait(false);
                var changeVector = session.Advanced.GetChangeVectorFor(game);

                if (game == null)
                {
                    throw new GameNotFoundException();
                }

                if ((game.InvitedPlayerId != null && game.InvitedPlayerId != player2Id) || game.Player2.PlayerId != null || game.Player1.PlayerId == player2Id)
                {
                    throw new ActionNotAllowedException("You are not allowed to join the requested game.");
                }

                game.UtcDateTimeStarted = _dateTimeProvider.GetUtcDateTime();
                game.Player2.PlayerId = player2Id;

                // TODO: Remove the display name from the command and retrieve from the identity data.
                game.Player2.DisplayName = command.PlayerDisplayName;
                game.Status = GameStatus.InProgress;

                // TODO: Investigate what exception is thrown when a concurrent update occurs (because of the changevector) and rethrow an appropriate custom exception
                await session.StoreAsync(game, changeVector, game.Id, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}