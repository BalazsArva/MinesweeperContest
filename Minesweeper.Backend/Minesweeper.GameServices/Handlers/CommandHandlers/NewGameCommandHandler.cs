using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Utilities.Generators;
using Minesweeper.GameServices.Utilities.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class NewGameCommandHandler : INewGameCommandHandler
    {
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;
        private readonly IPlayerMarksGenerator _playerMarksGenerator;
        private readonly IGuidProvider _guidProvider;

        public NewGameCommandHandler(IDocumentStore documentStore, IGameGenerator gameGenerator, IPlayerMarksGenerator playerMarksGenerator, IGuidProvider guidProvider)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
            _playerMarksGenerator = playerMarksGenerator ?? throw new ArgumentNullException(nameof(playerMarksGenerator));
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
                var gameId = _guidProvider.GenerateGuidString();
                var game = _gameGenerator.GenerateGame(command.TableRows, command.TableColumns, command.MineCount);

                var player1MarkTable = _playerMarksGenerator.GenerateDefaultPlayerMarksTable(command.TableRows, command.TableColumns);
                var player1MarksDocumentId = session.GetPlayerMarksDocumentId(gameId, command.HostPlayerId);
                var player1MarksDocument = new PlayerMarks { Marks = player1MarkTable, Id = player1MarksDocumentId };

                // TODO: Validate that the invited player's Id is not the same as the host's Id.
                game.Id = _documentStore.GetPrefixedDocumentId<Game>(gameId);
                game.InvitedPlayerId = string.IsNullOrWhiteSpace(command.InvitedPlayerId) ? null : command.InvitedPlayerId;
                game.Player1.PlayerId = command.HostPlayerId;
                game.Player1.DisplayName = command.HostPlayerDisplayName;

                await session.StoreAsync(game, cancellationToken).ConfigureAwait(false);
                await session.StoreAsync(player1MarksDocument, cancellationToken).ConfigureAwait(false);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return gameId;
            }
        }
    }
}