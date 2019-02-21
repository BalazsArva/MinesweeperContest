using System;
using System.Linq;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.DataStructures;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices
{
    public class GameDriver : IGameDriver
    {
        private const int PointsForMineFound = 10;
        private const int BonusPointsWindowSeconds = 5;

        private readonly IDateTimeProvider _dateTimeProvider;

        public GameDriver(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public MoveResultType MakeMove(Game game, string playerId, int row, int column)
        {
            var player = game.Player1.PlayerId == playerId ? Players.Player1 : Players.Player2;
            var openedFieldLookup = FieldLookup.FromGame(game);

            var playerAllowedToMove = CheckPlayerCanMove(game, player);
            if (!playerAllowedToMove)
            {
                return MoveResultType.NotYourTurn;
            }

            var canPerformMove = CheckCanPerformMove(game.GameTable, openedFieldLookup, row, column);
            if (!canPerformMove)
            {
                return MoveResultType.CannotMoveThere;
            }

            game.Status = GameStatus.InProgress;

            PerformMove(game, openedFieldLookup, player, row, column);

            var gameIsOver = GameIsOver(game);
            if (gameIsOver)
            {
                game.Status = GameStatus.Finished;
                game.Winner = DetermineWinner(game);

                return MoveResultType.GameOver;
            }

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

        private bool CheckCanPerformMove(GameTable gameTable, FieldLookup fieldLookup, int row, int column)
        {
            // Field is not yet open and within proper range
            return
                row >= 0 &&
                row < gameTable.Rows &&
                column >= 0 &&
                column < gameTable.Columns &&
                fieldLookup.IsFieldRegistered(row, column) == false;
        }

        private void PerformMove(Game game, FieldLookup fieldLookup, Players player, int row, int column)
        {
            var utcNow = _dateTimeProvider.GetUtcDateTime();
            var table = game.GameTable;

            if (table.FieldMatrix[row, column] == FieldTypes.Mined)
            {
                var points = GetPointsForMineFound(game, player, utcNow);

                AddPoints(game, player, points);
                RecordMove(game, fieldLookup, player, row, column, utcNow);

                return;
            }

            var neighboringMineCount = 0;
            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                var rowToCheck = row + rowOffset;

                // Row out of range
                if (rowToCheck < 0 || rowToCheck >= table.Rows)
                {
                    continue;
                }

                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    var colToCheck = column + colOffset;

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

            if (neighboringMineCount == 0)
            {
                DoRecursiveMove(game, fieldLookup, player, row, column, utcNow);
            }
            else
            {
                RecordMove(game, fieldLookup, player, row, column, utcNow);
            }
        }

        private void DoRecursiveMove(Game game, FieldLookup fieldLookup, Players player, int row, int column, DateTime utcDateTimeRecorded)
        {
            var table = game.GameTable;

            var rowOverflown = row < 0 || row >= table.Rows;
            var colOverflown = column < 0 || column >= table.Columns;
            var isMined = table.FieldMatrix[row, column] == FieldTypes.Mined;
            var isMovedOn = fieldLookup.IsFieldRegistered(row, column);

            if (rowOverflown || colOverflown || isMined || isMovedOn)
            {
                return;
            }

            RecordMove(game, fieldLookup, player, row, column, utcDateTimeRecorded);

            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    // The same field for which this recursion step was called for, skip.
                    if (rowOffset == 0 && colOffset == 0)
                    {
                        continue;
                    }

                    DoRecursiveMove(game, fieldLookup, player, row + rowOffset, column + colOffset, utcDateTimeRecorded);
                }
            }
        }

        private void RecordMove(Game game, FieldLookup fieldLookup, Players player, int row, int column, DateTime utcDateTimeRecorded)
        {
            fieldLookup.RegisterField(row, column);

            game.Moves.Add(new GameMove
            {
                Player = player,
                Row = row,
                Column = column,
                UtcDateTimeRecorded = utcDateTimeRecorded
            });
        }

        private void AddPoints(Game game, Players player, int points)
        {
            if (player == Players.Player1)
            {
                game.Player1.Points += points;
            }
            else
            {
                game.Player2.Points += points;
            }
        }

        private int GetPointsForMineFound(Game game, Players player, DateTime utcDateOfMove)
        {
            var basePoints = PointsForMineFound;
            var bonus = 0;

            if (game.Moves.Count > 0)
            {
                var lastMove = game.Moves.Last();
                var differenceSeconds = (int)Math.Floor((utcDateOfMove - lastMove.UtcDateTimeRecorded).TotalSeconds);

                if (differenceSeconds >= 0 && differenceSeconds <= BonusPointsWindowSeconds)
                {
                    bonus = BonusPointsWindowSeconds - differenceSeconds;
                }
            }

            return basePoints + bonus;
        }

        private bool GameIsOver(Game game)
        {
            var table = game.GameTable;

            return game.Moves.Count == table.Rows * table.Columns;
        }

        private Players? DetermineWinner(Game game)
        {
            var player1Points = game.Player1.Points;
            var player2Points = game.Player2.Points;

            if (player1Points == player2Points)
            {
                return null;
            }
            else if (player1Points > player2Points)
            {
                return Players.Player1;
            }

            return Players.Player2;
        }
    }
}