using System;
using Minesweeper.GameServices.Contracts;

namespace Minesweeper.GameServices.Converters
{
    public static class MarkTypeConverter
    {
        public static MarkTypes ToContract(GameModel.MarkTypes markType)
        {
            switch (markType)
            {
                case GameModel.MarkTypes.Empty:
                    return MarkTypes.Empty;

                case GameModel.MarkTypes.None:
                    return MarkTypes.None;

                case GameModel.MarkTypes.Unknown:
                    return MarkTypes.Unknown;

                default:
                    throw new ArgumentOutOfRangeException(nameof(markType), $"The value {(int)markType} is not valid for this parameter.");
            }
        }

        public static GameModel.MarkTypes FromContract(MarkTypes markType)
        {
            switch (markType)
            {
                case MarkTypes.Empty:
                    return GameModel.MarkTypes.Empty;

                case MarkTypes.None:
                    return GameModel.MarkTypes.None;

                case MarkTypes.Unknown:
                    return GameModel.MarkTypes.Unknown;

                default:
                    throw new ArgumentOutOfRangeException(nameof(markType), $"The value {(int)markType} is not valid for this parameter.");
            }
        }
    }
}