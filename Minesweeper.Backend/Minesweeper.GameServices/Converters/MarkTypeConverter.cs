using System;

namespace Minesweeper.GameServices.Converters
{
    public static class MarkTypeConverter
    {
        public static Contracts.MarkTypes ToContract(GameModel.MarkTypes markType)
        {
            switch (markType)
            {
                case GameModel.MarkTypes.Empty:
                    return Contracts.MarkTypes.Empty;

                case GameModel.MarkTypes.None:
                    return Contracts.MarkTypes.None;

                case GameModel.MarkTypes.Unknown:
                    return Contracts.MarkTypes.Unknown;

                default:
                    throw new ArgumentOutOfRangeException(nameof(markType), $"The value {(int)markType} is not valid for this parameter.");
            }
        }

        public static GameModel.MarkTypes FromContract(Contracts.MarkTypes markType)
        {
            switch (markType)
            {
                case Contracts.MarkTypes.Empty:
                    return GameModel.MarkTypes.Empty;

                case Contracts.MarkTypes.None:
                    return GameModel.MarkTypes.None;

                case Contracts.MarkTypes.Unknown:
                    return GameModel.MarkTypes.Unknown;

                default:
                    throw new ArgumentOutOfRangeException(nameof(markType), $"The value {(int)markType} is not valid for this parameter.");
            }
        }
    }
}