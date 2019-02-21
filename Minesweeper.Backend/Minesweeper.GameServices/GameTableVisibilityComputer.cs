using System.Linq;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices
{
    public class GameTableVisibilityComputer : IGameTableVisibilityComputer
    {
        public VisibleFieldType[,] GetVisibleGameTableAsync(Game game)
        {
            var table = game.GameTable;
            var visibleTable = new VisibleFieldType[table.Rows, table.Columns];

            for (var row = 0; row < table.Rows; ++row)
            {
                for (var col = 0; col < table.Columns; ++col)
                {
                    var moveOnField = game.Moves.FirstOrDefault(m => m.Column == col && m.Row == row);
                    var isMined = table.FieldMatrix[row, col] == FieldTypes.Mined;

                    if (isMined && moveOnField != null)
                    {
                        var type = moveOnField.Player == Players.Player1
                            ? VisibleFieldType.Player1FoundMine
                            : VisibleFieldType.Player2FoundMine;

                        visibleTable[row, col] = type;
                    }
                    else if (moveOnField != null)
                    {
                        var minesAroundField = GetMinesAroundField(game, row, col);

                        switch (minesAroundField)
                        {
                            case 0: visibleTable[row, col] = VisibleFieldType.MinesAround0; break;
                            case 1: visibleTable[row, col] = VisibleFieldType.MinesAround1; break;
                            case 2: visibleTable[row, col] = VisibleFieldType.MinesAround2; break;
                            case 3: visibleTable[row, col] = VisibleFieldType.MinesAround3; break;
                            case 4: visibleTable[row, col] = VisibleFieldType.MinesAround4; break;
                            case 5: visibleTable[row, col] = VisibleFieldType.MinesAround5; break;
                            case 6: visibleTable[row, col] = VisibleFieldType.MinesAround6; break;
                            case 7: visibleTable[row, col] = VisibleFieldType.MinesAround7; break;
                            case 8: visibleTable[row, col] = VisibleFieldType.MinesAround8; break;
                        }
                    }
                    else
                    {
                        visibleTable[row, col] = VisibleFieldType.Unknown;
                    }
                }
            }

            return visibleTable;
        }

        private int GetMinesAroundField(Game game, int row, int col)
        {
            var table = game.GameTable;

            var mineCount = 0;
            for (var rowOffset = -1; rowOffset <= 1; ++rowOffset)
            {
                for (var colOffset = -1; colOffset <= 1; ++colOffset)
                {
                    var rowToCheck = row + rowOffset;
                    var colToCheck = col + colOffset;

                    if (rowToCheck < 0 || rowToCheck >= table.Rows)
                    {
                        continue;
                    }

                    if (colToCheck < 0 || colToCheck >= table.Columns)
                    {
                        continue;
                    }

                    if (table.FieldMatrix[rowToCheck, colToCheck] == FieldTypes.Mined)
                    {
                        ++mineCount;
                    }
                }
            }

            return mineCount;
        }
    }
}