using System;
using System.Linq;

namespace Minesweeper.GameServices.Extensions
{
    public static class LinqExtensions
    {
        public static int Count<T>(this T[][] matrix, Func<T, bool> predicate)
        {
            if (matrix == null)
            {
                throw new ArgumentNullException(nameof(matrix));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return matrix.Sum(row => row.Count(predicate));
        }
    }
}