using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class JoinGameCommandHandler : IJoinGameCommandHandler
    {
        private readonly IDocumentStore _documentStore;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPlayerMarksGenerator _playerMarksGenerator;

        public JoinGameCommandHandler(IDocumentStore documentStore, IPlayerMarksGenerator playerMarksGenerator, IDateTimeProvider dateTimeProvider)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _playerMarksGenerator = playerMarksGenerator ?? throw new ArgumentNullException(nameof(playerMarksGenerator));
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

                var player2MarkTable = _playerMarksGenerator.GenerateDefaultPlayerMarksTable(game.Rows, game.Columns);
                var player2MarksDocumentId = session.GetPlayerMarksDocumentId(command.GameId, command.PlayerId);
                var player2MarksDocument = new PlayerMarks { Marks = player2MarkTable, Id = player2MarksDocumentId };

                game.UtcDateTimeStarted = _dateTimeProvider.GetUtcDateTime();
                game.Player2.PlayerId = player2Id;

                // TODO: Remove the display name from the command and retrieve from the identity data.
                game.Player2.DisplayName = command.PlayerDisplayName;
                game.Status = GameStatus.InProgress;

                // TODO: Investigate what exception is thrown when a concurrent update occurs (because of the changevector) and rethrow an appropriate custom exception
                await session.StoreAsync(game, changeVector, game.Id, cancellationToken).ConfigureAwait(false);
                await session.StoreAsync(player2MarksDocument, cancellationToken).ConfigureAwait(false);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}