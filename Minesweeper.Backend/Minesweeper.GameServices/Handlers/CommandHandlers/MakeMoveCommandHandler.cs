using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Contracts.Notifications;
using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.Converters;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public class MakeMoveCommandHandler : IMakeMoveCommandHandler
    {
        private readonly IMediator _mediator;
        private readonly IDocumentStore _documentStore;
        private readonly IGameDriver _gameDriver;

        public MakeMoveCommandHandler(IMediator mediator, IDocumentStore documentStore, IGameDriver gameDriver)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameDriver = gameDriver ?? throw new ArgumentNullException(nameof(gameDriver));
        }

        public async Task<MoveResult> HandleAsync(MakeMoveCommand command, CancellationToken cancellationToken)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Concurrency protection
                var gameId = command.GameId;
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var currentPlayerId = GetPlayerIdByPlayerOrder(game, game.NextPlayer);
                var currentVisibleTable = EnumArrayCloner.Clone(game.VisibleTable);
                var mineCountBeforeMove = GetRemainingMineCount(game);

                var movementResult = _gameDriver.MakeMove(game, command.PlayerId, command.Row, command.Column);

                if (movementResult == MoveResultType.Success || movementResult == MoveResultType.GameOver)
                {
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    var mineCountAfterMove = GetRemainingMineCount(game);
                    var nextPlayerId = GetPlayerIdByPlayerOrder(game, game.NextPlayer);

                    await PublishTableUpdatedAsync(gameId, currentVisibleTable, game.VisibleTable, cancellationToken);
                    await PublishPlayersTurnIfChangedAsync(gameId, currentPlayerId, nextPlayerId, cancellationToken).ConfigureAwait(false);
                    await PublishRemainingMinesIfChangedAsync(gameId, mineCountBeforeMove, mineCountAfterMove, cancellationToken).ConfigureAwait(false);
                    await PublishWinnerIfGameIsOverAsync(gameId, game.Winner, game.Player1.PlayerId, game.Player2.PlayerId, cancellationToken).ConfigureAwait(false);
                }

                return new MoveResult { MoveResultType = movementResult };
            }
        }

        private async Task PublishTableUpdatedAsync(string gameId, GameModel.VisibleFieldType[][] previousTableState, GameModel.VisibleFieldType[][] newTableState, CancellationToken cancellationToken)
        {
            var fieldUpdates = new List<FieldUpdate>(previousTableState.Length * previousTableState[0].Length);

            for (var row = 0; row < previousTableState.Length; ++row)
            {
                for (var col = 0; col < previousTableState[0].Length; ++col)
                {
                    if (previousTableState[row][col] != newTableState[row][col])
                    {
                        var fieldType = FieldTypeConverter.ToContract(newTableState[row][col]);

                        fieldUpdates.Add(new FieldUpdate(row, col, fieldType));
                    }
                }
            }

            await _mediator.Publish(new GameTableUpdatedNotification(gameId, fieldUpdates), cancellationToken).ConfigureAwait(false);
        }

        private async Task PublishPlayersTurnIfChangedAsync(string gameId, string playerIdBeforeMove, string playerIdAfterMove, CancellationToken cancellationToken)
        {
            if (playerIdBeforeMove != playerIdAfterMove)
            {
                await _mediator.Publish(new PlayersTurnNotification(gameId, playerIdAfterMove), cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task PublishRemainingMinesIfChangedAsync(string gameId, int mineCountBeforeMove, int mineCountAfterMove, CancellationToken cancellationToken)
        {
            if (mineCountBeforeMove != mineCountAfterMove)
            {
                await _mediator.Publish(new RemainingMinesChangedNotification(gameId, mineCountAfterMove)).ConfigureAwait(false);
            }
        }

        private async Task PublishWinnerIfGameIsOverAsync(string gameId, Players? winnerPlayer, string player1Id, string player2Id, CancellationToken cancellationToken)
        {
            if (winnerPlayer.HasValue)
            {
                var winner = winnerPlayer.Value;
                var winnerPlayerId = winner == Players.Player1 ? player1Id : player2Id;

                await _mediator.Publish(new GameOverNotification(gameId, winnerPlayerId), cancellationToken).ConfigureAwait(false);
            }
        }

        private int GetRemainingMineCount(Game game)
        {
            var foundMineCount = game.VisibleTable.Count(field => field == GameModel.VisibleFieldType.Player1FoundMine || field == GameModel.VisibleFieldType.Player2FoundMine);
            var totalMineCount = game.Mines;

            return totalMineCount - foundMineCount;
        }

        private string GetPlayerIdByPlayerOrder(Game game, Players player)
        {
            return player == Players.Player1
                ? game.Player1.PlayerId
                : game.Player2.PlayerId;
        }
    }
}