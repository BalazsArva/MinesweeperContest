using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class MarkFieldCommandHandler : IMarkFieldCommandHandler
    {
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;
        private readonly IGuidProvider _guidProvider;

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

                // TODO: Index overflow and underflow check here and everywhere else
                if (isPlayer1)
                {
                    session.Advanced.Patch<Game, GameModel.MarkTypes>(game.Id, g => g.Player1Marks[command.Row][command.Column], newMark);
                }
                else
                {
                    session.Advanced.Patch<Game, GameModel.MarkTypes>(game.Id, g => g.Player2Marks[command.Row][command.Column], newMark);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}