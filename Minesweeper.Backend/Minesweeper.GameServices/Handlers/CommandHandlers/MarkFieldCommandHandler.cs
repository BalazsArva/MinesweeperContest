using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class MarkFieldCommandHandler : IMarkFieldCommandHandler
    {
        private readonly IDocumentStore _documentStore;

        public MarkFieldCommandHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task HandleAsync(MarkFieldCommand command, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(command.GameId, cancellationToken).ConfigureAwait(false);

                if (game.Player1.PlayerId != command.PlayerId && game.Player2.PlayerId != command.PlayerId)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                var isPlayer1 = game.Player1.PlayerId == command.PlayerId;
                var newMark = MarkTypeConverter.FromContract(command.MarkType);
                var row = command.Row;
                var col = command.Column;

                // TODO: Index overflow and underflow check here and everywhere else
                // The 'row' and 'col' variables must exist as they are. For some reason, if command.Row / command.Column
                // is used in the Patch expression's indexer (i.e. g.Player1Marks[command.Row][command.Column]) it fails to translate.
                if (isPlayer1)
                {
                    session.Advanced.Patch<Game, GameModel.MarkTypes>(game.Id, g => g.Player1Marks[row][col], newMark);
                }
                else
                {
                    session.Advanced.Patch<Game, GameModel.MarkTypes>(game.Id, g => g.Player2Marks[row][col], newMark);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}