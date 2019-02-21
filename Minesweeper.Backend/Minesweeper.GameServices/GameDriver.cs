using System;
using System.Linq;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices
{
    public class GameDriver : IGameDriver
    {
        public MoveResultType MakeMove(Game game, string playerId, int row, int column)
        {
            var isPlayer1 = game.Player1.PlayerId == playerId;
            var player = isPlayer1 ? Players.Player1 : Players.Player2;

            var playerAllowedToMove = CheckPlayerCanMove(game, player);
            if (!playerAllowedToMove)
            {
                return MoveResultType.NotYourTurn;
            }

            var canPerformMove = CheckCanPerformMove(game, row, column);
            if (!canPerformMove)
            {
                return MoveResultType.CannotMoveThere;
            }

            PerformMove(game, player, row, column);

            return MoveResultType.Success;
        }

        private bool CheckPlayerCanMove(Game game, Players player)
        {
            var isFirstMove = game.Moves.Count == 0;

            if (isFirstMove)
            {
                return game.StarterPlayer == player;
            }
            else
            {
                var lastMove = game.Moves.Last();
                var lastMoveWasOnMine = game.GameTable.FieldMatrix[lastMove.Row, lastMove.Column] == FieldTypes.Mined;

                if (lastMoveWasOnMine)
                {
                    // If last move was on mine, the player who did that move is allowed to do the next as well.
                    return lastMove.Player == player;
                }
                else
                {
                    // Otherwise it's the other player's turn.
                    return lastMove.Player != player;
                }
            }
        }

        private bool CheckCanPerformMove(Game game, int row, int column)
        {
            var table = game.GameTable;

            // Field is not yet open and within proper range
            return
                row >= 0 &&
                row < table.Rows &&
                column >= 0 &&
                column < table.Columns &&
                game.Moves.All(move => move.Row != row || move.Column != column);
        }

        private void PerformMove(Game game, Players player, int row, int column)
        {
            var table = game.GameTable;

            var neighboringMineCount = 0;
            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    var rowToCheck = row + rowOffset;
                    var colToCheck = column + colOffset;

                    // Row out of range
                    if (rowToCheck < 0 || rowToCheck >= table.Rows)
                    {
                        continue;
                    }

                    // Column out of range
                    if (colToCheck < 0 || colToCheck >= table.Columns)
                    {
                        continue;
                    }

                    if (table.FieldMatrix[rowToCheck, colToCheck] == FieldTypes.Mined)
                    {
                        ++neighboringMineCount;
                    }
                }
            }

            var utcNow = DateTime.UtcNow;
            if (neighboringMineCount == 0)
            {
                DoRecursiveMove(game, player, row, column, utcNow);
            }
            else
            {
                game.Moves.Add(new GameMove
                {
                    Player = player,
                    Row = row,
                    Column = column,
                    UtcDateTimeRecorded = utcNow
                });
            }
        }

        private void DoRecursiveMove(Game game, Players player, int row, int column, DateTime utcDateTimeRecorded)
        {
            var table = game.GameTable;

            if (row < 0 || row >= table.Rows)
            {
                return;
            }

            if (column < 0 || column >= table.Columns)
            {
                return;
            }

            // TODO: Use a faster structure to look up opened fields
            if (game.Moves.Any(m => m.Row == row && m.Column == column))
            {
                return;
            }

            game.Moves.Add(new GameMove
            {
                Player = player,
                Row = row,
                Column = column,
                UtcDateTimeRecorded = utcDateTimeRecorded
            });

            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    if (rowOffset == 0 && colOffset == 0)
                    {
                        continue;
                    }

                    DoRecursiveMove(game, player, row + rowOffset, column + colOffset, utcDateTimeRecorded);
                }
            }
        }
    }
}