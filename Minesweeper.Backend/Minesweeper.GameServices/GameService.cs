﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;
        private readonly IGameDriver _gameDriver;
        private readonly IGuidProvider _guidProvider;
        private readonly IGameTableVisibilityComputer _gameTableVisibilityComputer;

        public GameService(IDocumentStore documentStore, IGameGenerator gameGenerator, IGameDriver gameDriver, IGuidProvider guidProvider, IGameTableVisibilityComputer gameTableVisibilityComputer)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
            _gameDriver = gameDriver ?? throw new ArgumentNullException(nameof(gameDriver));
            _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
            _gameTableVisibilityComputer = gameTableVisibilityComputer ?? throw new ArgumentNullException(nameof(gameTableVisibilityComputer));
        }

        public async Task<NewGameInfo> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = _gameGenerator.GenerateGame(tableRows, tableColumns, mineCount);

                game.Id = _documentStore.GetPrefixedDocumentId<Game>(_guidProvider.GenerateGuidString());
                game.Player1 = new Player
                {
                    PlayerId = hostPlayerId,
                    DisplayName = hostPlayerDisplayName,
                    Points = 0
                };

                await session.StoreAsync(game, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new NewGameInfo(game.Id, game.EntryToken);
            }
        }

        public async Task JoinGameAsync(string gameId, string player2Id, string playerDisplayName, string entryToken, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                // TODO: Validate: Player2 is unset, entry token is valid, game not finished, etc.
                game.Player2 = new Player
                {
                    PlayerId = player2Id,
                    DisplayName = playerDisplayName,
                    Points = 0
                };

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var movementResult = _gameDriver.MakeMove(game, playerId, row, column);
                if (movementResult == MoveResultType.Success || movementResult == MoveResultType.GameOver)
                {
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }

                return new MoveResult { MoveResultType = movementResult };
            }
        }

        public async Task<VisibleFieldType[,]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                return _gameTableVisibilityComputer.GetVisibleGameTableAsync(game);
            }
        }
    }
}