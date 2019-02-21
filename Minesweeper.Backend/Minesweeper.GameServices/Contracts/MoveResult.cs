namespace Minesweeper.GameServices.Contracts
{
    public class MoveResult
    {
        public MoveResultType MoveResultType { get; set; }
    }

    public enum MoveResultType
    {
        Success,

        NotYourTurn,

        CannotMoveThere
    }
}