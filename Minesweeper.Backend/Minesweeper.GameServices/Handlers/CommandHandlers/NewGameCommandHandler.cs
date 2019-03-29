using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class NewGameCommandHandler : INewGameCommandHandler
    {
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;
        private readonly IGuidProvider _guidProvider;

        public NewGameCommandHandler(IDocumentStore documentStore, IGameGenerator gameGenerator, IGuidProvider guidProvider)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
            _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        }

        public async Task<string> HandleAsync(NewGameCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = _gameGenerator.GenerateGame(command.TableRows, command.TableColumns, command.MineCount);

                // TODO: Validate that the invited player's Id is not the same as the host's Id.
                game.Id = _documentStore.GetPrefixedDocumentId<Game>(_guidProvider.GenerateGuidString());
                game.InvitedPlayerId = string.IsNullOrWhiteSpace(command.InvitedPlayerId) ? null : command.InvitedPlayerId;
                game.Player1.PlayerId = command.HostPlayerId;

                // TODO: Look up the real display name using some user API
                game.Player1.DisplayName = command.HostPlayerId;

                await session.StoreAsync(game, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return game.Id;
            }
        }
    }
}