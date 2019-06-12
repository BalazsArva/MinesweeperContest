using System;
using System.Threading;
using System.Threading.Tasks;
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
                var marks = await session.LoadPlayerMarksAsync(command.GameId, command.PlayerId, cancellationToken).ConfigureAwait(false);

                if (marks == null)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                var newMark = MarkTypeConverter.FromContract(command.MarkType);
                var row = command.Row;
                var col = command.Column;

                // TODO: Index overflow and underflow check here and everywhere else
                // The 'row' and 'col' variables must exist as they are. For some reason, if command.Row / command.Column
                // is used in the Patch expression's indexer (i.e. g.Player1Marks[command.Row][command.Column]) it fails to translate.
                session.Advanced.Patch<PlayerMarks, MarkTypes>(marks.Id, g => g.Marks[row][col], newMark);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}