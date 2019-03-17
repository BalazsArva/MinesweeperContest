using System;

namespace Minesweeper.GameServices.Converters
{
    public static class FieldTypeConverter
    {
        public static Contracts.VisibleFieldType ToContract(GameModel.VisibleFieldType visibleFieldType)
        {
            switch (visibleFieldType)
            {
                case GameModel.VisibleFieldType.MinesAround0:
                    return Contracts.VisibleFieldType.MinesAround0;

                case GameModel.VisibleFieldType.MinesAround1:
                    return Contracts.VisibleFieldType.MinesAround1;

                case GameModel.VisibleFieldType.MinesAround2:
                    return Contracts.VisibleFieldType.MinesAround2;

                case GameModel.VisibleFieldType.MinesAround3:
                    return Contracts.VisibleFieldType.MinesAround3;

                case GameModel.VisibleFieldType.MinesAround4:
                    return Contracts.VisibleFieldType.MinesAround4;

                case GameModel.VisibleFieldType.MinesAround5:
                    return Contracts.VisibleFieldType.MinesAround5;

                case GameModel.VisibleFieldType.MinesAround6:
                    return Contracts.VisibleFieldType.MinesAround6;

                case GameModel.VisibleFieldType.MinesAround7:
                    return Contracts.VisibleFieldType.MinesAround7;

                case GameModel.VisibleFieldType.MinesAround8:
                    return Contracts.VisibleFieldType.MinesAround8;

                case GameModel.VisibleFieldType.Player1FoundMine:
                    return Contracts.VisibleFieldType.Player1FoundMine;

                case GameModel.VisibleFieldType.Player2FoundMine:
                    return Contracts.VisibleFieldType.Player2FoundMine;

                case GameModel.VisibleFieldType.Unknown:
                    return Contracts.VisibleFieldType.Unknown;

                default:
                    throw new ArgumentOutOfRangeException(nameof(visibleFieldType), $"The value {(int)visibleFieldType} is not valid for this parameter.");
            }
        }
    }
}