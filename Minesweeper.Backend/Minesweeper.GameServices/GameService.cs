﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IMediator _mediator;
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;
        private readonly IGameDriver _gameDriver;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuidProvider _guidProvider;

        public GameService(IMediator mediator, IDocumentStore documentStore, IGameGenerator gameGenerator, IGameDriver gameDriver, IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
            _gameDriver = gameDriver ?? throw new ArgumentNullException(nameof(gameDriver));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        }

        public async Task<string> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, string invitedPlayerId, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = _gameGenerator.GenerateGame(tableRows, tableColumns, mineCount);

                game.Id = _documentStore.GetPrefixedDocumentId<Game>(_guidProvider.GenerateGuidString());
                game.InvitedPlayerId = string.IsNullOrWhiteSpace(invitedPlayerId) ? null : invitedPlayerId;
                game.Player1.PlayerId = hostPlayerId;
                game.Player1.DisplayName = hostPlayerDisplayName;

                await session.StoreAsync(game, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return game.Id;
            }
        }

        public async Task JoinGameAsync(string gameId, string player2Id, string player2DisplayName, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);
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
                game.Player2.DisplayName = player2DisplayName;

                // TODO: Investigate what exception is thrown when a concurrent update occurs (because of the changevector) and rethrow an appropriate custom exception
                await session.StoreAsync(game, changeVector, game.Id, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Concurrency protection
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var currentPlayer = game.NextPlayer;
                var currentVisibleTable = CloneVisibleFields(game.VisibleTable);

                var movementResult = _gameDriver.MakeMove(game, playerId, row, column);

                if (movementResult == MoveResultType.Success || movementResult == MoveResultType.GameOver)
                {
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    await PublishTableUpdatedAsync(gameId, currentVisibleTable, game.VisibleTable, cancellationToken);
                }

                if (movementResult == MoveResultType.Success && currentPlayer != game.NextPlayer)
                {
                    var nextPlayerId = game.NextPlayer == Players.Player1 ? game.Player1.PlayerId : game.Player2.PlayerId;

                    await PublishPlayersTurnAsync(gameId, nextPlayerId, cancellationToken).ConfigureAwait(false);
                }

                return new MoveResult { MoveResultType = movementResult };
            }
        }

        public async Task MarkFieldAsync(string gameId, string playerId, int row, int column, MarkType markType, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                if (game.Player1.PlayerId != playerId && game.Player2.PlayerId != playerId)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                // TODO: Index overflow and underflow check here and everywhere else
                var isPlayer1 = game.Player1.PlayerId == playerId;

                var newMark = MapContractMarkTypeToEntityMarkType(markType);

                if (isPlayer1)
                {
                    session.Advanced.Patch<Game, MarkTypes>(game.Id, g => g.Player1Marks[row][column], newMark);
                }
                else
                {
                    session.Advanced.Patch<Game, MarkTypes>(game.Id, g => g.Player2Marks[row][column], newMark);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<MarkType[,]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken)
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
                var rows = playerMarks.Length;
                var columns = playerMarks[0].Length;

                var result = new MarkType[rows, columns];

                for (var row = 0; row < rows; ++row)
                {
                    for (var col = 0; col < columns; ++col)
                    {
                        result[row, col] = MapEntityMarkTypeToContractMarkType(playerMarks[row][col]);
                    }
                }

                return result;
            }
        }

        public async Task<Contracts.VisibleFieldType[,]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Validate user id
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var result = new Contracts.VisibleFieldType[game.Rows, game.Columns];
                for (var row = 0; row < game.Rows; ++row)
                {
                    for (var col = 0; col < game.Columns; ++col)
                    {
                        result[row, col] = MapEntityVisibleFieldTypeToContractFieldType(game.VisibleTable[row][col]);
                    }
                }

                return result;
            }
        }

        private async Task PublishTableUpdatedAsync(string gameId, GameModel.VisibleFieldType[][] previousTableState, GameModel.VisibleFieldType[][] newTableState, CancellationToken cancellationToken)
        {
            var fieldUpdates = new List<GameTableUpdatedNotification.FieldUpdate>(previousTableState.Length * previousTableState[0].Length);

            for (var row = 0; row < previousTableState.Length; ++row)
            {
                for (var col = 0; col < previousTableState[0].Length; ++col)
                {
                    if (previousTableState[row][col] != newTableState[row][col])
                    {
                        var fieldType = MapEntityVisibleFieldTypeToContractFieldType(newTableState[row][col]);

                        fieldUpdates.Add(new GameTableUpdatedNotification.FieldUpdate(row, col, fieldType));
                    }
                }
            }

            await _mediator.Publish(new GameTableUpdatedNotification(gameId, fieldUpdates), cancellationToken);
        }

        private async Task PublishPlayersTurnAsync(string gameId, string nextPlayerId, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new PlayersTurnNotification(gameId, nextPlayerId), cancellationToken);
        }

        private MarkType MapEntityMarkTypeToContractMarkType(MarkTypes markType)
        {
            return markType == MarkTypes.Empty
                ? MarkType.Empty
                : markType == MarkTypes.None
                    ? MarkType.None
                    : MarkType.Unknown;
        }

        private MarkTypes MapContractMarkTypeToEntityMarkType(MarkType markType)
        {
            return markType == MarkType.Empty
                ? MarkTypes.Empty
                : markType == MarkType.None
                    ? MarkTypes.None
                    : MarkTypes.Unknown;
        }

        private Contracts.VisibleFieldType MapEntityVisibleFieldTypeToContractFieldType(GameModel.VisibleFieldType visibleFieldType)
        {
            switch (visibleFieldType)
            {
                case GameModel.VisibleFieldType.MinesAround0: return Contracts.VisibleFieldType.MinesAround0;
                case GameModel.VisibleFieldType.MinesAround1: return Contracts.VisibleFieldType.MinesAround1;
                case GameModel.VisibleFieldType.MinesAround2: return Contracts.VisibleFieldType.MinesAround2;
                case GameModel.VisibleFieldType.MinesAround3: return Contracts.VisibleFieldType.MinesAround3;
                case GameModel.VisibleFieldType.MinesAround4: return Contracts.VisibleFieldType.MinesAround4;
                case GameModel.VisibleFieldType.MinesAround5: return Contracts.VisibleFieldType.MinesAround5;
                case GameModel.VisibleFieldType.MinesAround6: return Contracts.VisibleFieldType.MinesAround6;
                case GameModel.VisibleFieldType.MinesAround7: return Contracts.VisibleFieldType.MinesAround7;
                case GameModel.VisibleFieldType.MinesAround8: return Contracts.VisibleFieldType.MinesAround8;
                case GameModel.VisibleFieldType.Player1FoundMine: return Contracts.VisibleFieldType.Player1FoundMine;
                case GameModel.VisibleFieldType.Player2FoundMine: return Contracts.VisibleFieldType.Player2FoundMine;
                case GameModel.VisibleFieldType.Unknown: return Contracts.VisibleFieldType.Unknown;
                default:
                    throw new ArgumentOutOfRangeException(nameof(visibleFieldType), $"The value {(int)visibleFieldType} is not valid for this parameter.");
            }
        }

        private GameModel.VisibleFieldType[][] CloneVisibleFields(GameModel.VisibleFieldType[][] visibleFields)
        {
            var result = new GameModel.VisibleFieldType[visibleFields.Length][];

            for (var row = 0; row < visibleFields.Length; ++row)
            {
                result[row] = new GameModel.VisibleFieldType[visibleFields[row].Length];

                for (var col = 0; col < visibleFields[row].Length; ++col)
                {
                    result[row][col] = visibleFields[row][col];
                }
            }

            return result;
        }
    }
}