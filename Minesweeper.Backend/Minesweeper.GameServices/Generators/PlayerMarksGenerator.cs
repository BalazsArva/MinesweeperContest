using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Generators
{
    public class PlayerMarksGenerator : IPlayerMarksGenerator
    {
        public MarkTypes[][] GenerateDefaultPlayerMarksTable(int tableRows, int tableColumns)
        {
            var marks = new MarkTypes[tableRows][];

            for (var row = 0; row < tableRows; ++row)
            {
                marks[row] = new MarkTypes[tableColumns];

                for (var col = 0; col < tableColumns; ++col)
                {
                    marks[row][col] = MarkTypes.None;
                }
            }

            return marks;
        }
    }
}