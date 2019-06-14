using System.Linq;
using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.GameEngine
{
    public class GameDriver : IGameDriver
    {
        private const int MaxStreakBonus = 1024;
        private const int PointsForMineFound = 10;

        public MoveResultType MakeMove(Game game, string playerId, int row, int column)
        {
            var player = game.Player1.PlayerId == playerId ? GameModel.Players.Player1 : GameModel.Players.Player2;

            if (!CheckPlayerCanMove(game, player))
            {
                return MoveResultType.NotYourTurn;
            }

            if (!CheckCanPerformMove(game, row, column))
            {
                return MoveResultType.CannotMoveThere;
            }

            game.Status = GameModel.GameStatus.InProgress;

            PerformMove(game, player, row, column);

            if (IsGameOver(game))
            {
                game.Status = GameModel.GameStatus.Finished;
                game.Winner = DetermineWinner(game);

                return MoveResultType.GameOver;
            }

            return MoveResultType.Success;
        }

        private bool CheckPlayerCanMove(Game game, GameModel.Players player)
        {
            if (game.Status == GameModel.GameStatus.Finished || game.Status == GameModel.GameStatus.NotStarted)
            {
                return false;
            }

            return game.NextPlayer == player;
        }

        private bool CheckCanPerformMove(Game game, int row, int column)
        {
            // Field is not yet open and within proper range
            return
                row >= 0 &&
                row < game.Rows &&
                column >= 0 &&
                column < game.Columns &&
                game.VisibleTable[row][column] == GameModel.VisibleFieldType.Unknown;
        }

        private void PerformMove(Game game, GameModel.Players player, int row, int column)
        {
            if (game.BaseTable[row][column] == FieldTypes.Mined)
            {
                AddPoints(game, player);
                RecordMove(game, player, row, column);

                return;
            }

            ResetStreakBonus(game, player);

            var neighboringMineCount = GetMinesAroundField(game, row, column);
            if (neighboringMineCount == 0)
            {
                DoRecursiveMove(game, player, row, column);
            }
            else
            {
                RecordMove(game, player, row, column);
            }

            game.NextPlayer = player == GameModel.Players.Player1
                ? GameModel.Players.Player2
                : GameModel.Players.Player1;
        }

        private void DoRecursiveMove(Game game, GameModel.Players player, int row, int column)
        {
            var rowOverflown = row < 0 || row >= game.Rows;
            var colOverflown = column < 0 || column >= game.Columns;

            if (rowOverflown || colOverflown)
            {
                return;
            }

            var isMined = game.BaseTable[row][column] == FieldTypes.Mined;
            var isMovedOn = game.VisibleTable[row][column] != GameModel.VisibleFieldType.Unknown;

            if (isMined || isMovedOn)
            {
                return;
            }

            RecordMove(game, player, row, column);

            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    // The same field for which this recursion step was called for, skip.
                    if (rowOffset == 0 && colOffset == 0)
                    {
                        continue;
                    }

                    var recursionRow = row + rowOffset;
                    var recursionCol = column + colOffset;

                    var hasZeroMinesAround = GetMinesAroundField(game, row, column) == 0;
                    if (hasZeroMinesAround)
                    {
                        DoRecursiveMove(game, player, recursionRow, recursionCol);
                    }
                }
            }
        }

        private void RecordMove(Game game, GameModel.Players player, int row, int col)
        {
            if (game.BaseTable[row][col] == FieldTypes.Mined)
            {
                game.VisibleTable[row][col] = player == GameModel.Players.Player1
                    ? GameModel.VisibleFieldType.Player1FoundMine
                    : GameModel.VisibleFieldType.Player2FoundMine;
            }
            else
            {
                var minesAround = GetMinesAroundField(game, row, col);
                switch (minesAround)
                {
                    case 0:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround0;
                        break;

                    case 1:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround1;
                        break;

                    case 2:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround2;
                        break;

                    case 3:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround3;
                        break;

                    case 4:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround4;
                        break;

                    case 5:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround5;
                        break;

                    case 6:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround6;
                        break;

                    case 7:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround7;
                        break;

                    case 8:
                        game.VisibleTable[row][col] = GameModel.VisibleFieldType.MinesAround8;
                        break;
                }
            }
        }

        private void ResetStreakBonus(Game game, GameModel.Players player)
        {
            var targetPlayer = player == GameModel.Players.Player1 ? game.Player1 : game.Player2;

            targetPlayer.StreakBonus = 0;
        }

        private void AddPoints(Game game, GameModel.Players player)
        {
            var targetPlayer = player == GameModel.Players.Player1 ? game.Player1 : game.Player2;
            var bonus = targetPlayer.StreakBonus;

            targetPlayer.Points += PointsForMineFound;

            if (bonus == 0)
            {
                targetPlayer.StreakBonus = 1;
            }
            else
            {
                targetPlayer.Points += bonus;
                targetPlayer.StreakBonus *= 2;

                if (targetPlayer.StreakBonus > MaxStreakBonus)
                {
                    targetPlayer.StreakBonus = MaxStreakBonus;
                }
            }
        }

        private int GetMinesAroundField(Game game, int row, int column)
        {
            var neighboringMineCount = 0;
            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                var rowToCheck = row + rowOffset;

                // Row out of range
                if (rowToCheck < 0 || rowToCheck >= game.Rows)
                {
                    continue;
                }

                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    var colToCheck = column + colOffset;

                    // Column out of range
                    if (colToCheck < 0 || colToCheck >= game.Columns)
                    {
                        continue;
                    }

                    if (game.BaseTable[rowToCheck][colToCheck] == FieldTypes.Mined)
                    {
                        ++neighboringMineCount;
                    }
                }
            }

            return neighboringMineCount;
        }

        private bool IsGameOver(Game game)
        {
            var foundMines = game.VisibleTable.Sum(arr => arr.Count(field => field == GameModel.VisibleFieldType.Player1FoundMine || field == GameModel.VisibleFieldType.Player2FoundMine));

            return foundMines == game.Mines;
        }

        private GameModel.Players? DetermineWinner(Game game)
        {
            var player1Points = game.Player1.Points;
            var player2Points = game.Player2.Points;

            if (player1Points == player2Points)
            {
                return null;
            }
            else if (player1Points > player2Points)
            {
                return GameModel.Players.Player1;
            }

            return GameModel.Players.Player2;
        }
    }
}