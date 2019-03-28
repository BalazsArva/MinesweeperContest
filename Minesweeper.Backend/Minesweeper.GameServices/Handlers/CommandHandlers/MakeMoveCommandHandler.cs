using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Commands;
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
                var game = await session.LoadGameAsync(command.GameId, cancellationToken).ConfigureAwait(false);

                var currentPlayer = game.NextPlayer;
                var currentVisibleTable = EnumArrayCloner.Clone(game.VisibleTable);

                var movementResult = _gameDriver.MakeMove(game, command.PlayerId, command.Row, command.Column);

                if (movementResult == MoveResultType.Success || movementResult == MoveResultType.GameOver)
                {
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    await PublishTableUpdatedAsync(command.GameId, currentVisibleTable, game.VisibleTable, cancellationToken);
                }

                if (movementResult == MoveResultType.Success && currentPlayer != game.NextPlayer)
                {
                    var nextPlayerId = game.NextPlayer == Players.Player1 ? game.Player1.PlayerId : game.Player2.PlayerId;

                    await PublishPlayersTurnAsync(command.GameId, nextPlayerId, cancellationToken).ConfigureAwait(false);
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

            await _mediator.Publish(new GameTableUpdatedNotification(gameId, fieldUpdates), cancellationToken);
        }

        private async Task PublishPlayersTurnAsync(string gameId, string nextPlayerId, CancellationToken cancellationToken)
        {
            await _mediator.Publish(new PlayersTurnNotification(gameId, nextPlayerId), cancellationToken);
        }
    }
}