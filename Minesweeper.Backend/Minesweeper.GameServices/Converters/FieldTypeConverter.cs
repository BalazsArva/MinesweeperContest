using System;
using Minesweeper.GameServices.Contracts.Responses.Game;

namespace Minesweeper.GameServices.Converters
{
    public static class FieldTypeConverter
    {
        public static VisibleFieldType ToContract(GameModel.VisibleFieldType visibleFieldType)
        {
            switch (visibleFieldType)
            {
                case GameModel.VisibleFieldType.MinesAround0:
                    return VisibleFieldType.MinesAround0;

                case GameModel.VisibleFieldType.MinesAround1:
                    return VisibleFieldType.MinesAround1;

                case GameModel.VisibleFieldType.MinesAround2:
                    return VisibleFieldType.MinesAround2;

                case GameModel.VisibleFieldType.MinesAround3:
                    return VisibleFieldType.MinesAround3;

                case GameModel.VisibleFieldType.MinesAround4:
                    return VisibleFieldType.MinesAround4;

                case GameModel.VisibleFieldType.MinesAround5:
                    return VisibleFieldType.MinesAround5;

                case GameModel.VisibleFieldType.MinesAround6:
                    return VisibleFieldType.MinesAround6;

                case GameModel.VisibleFieldType.MinesAround7:
                    return VisibleFieldType.MinesAround7;

                case GameModel.VisibleFieldType.MinesAround8:
                    return VisibleFieldType.MinesAround8;

                case GameModel.VisibleFieldType.Player1FoundMine:
                    return VisibleFieldType.Player1FoundMine;

                case GameModel.VisibleFieldType.Player2FoundMine:
                    return VisibleFieldType.Player2FoundMine;

                case GameModel.VisibleFieldType.Unknown:
                    return VisibleFieldType.Unknown;

                default:
                    throw new ArgumentOutOfRangeException(nameof(visibleFieldType), $"The value {(int)visibleFieldType} is not valid for this parameter.");
            }
        }
    }
}