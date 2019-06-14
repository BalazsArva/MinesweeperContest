using System;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Utilities.Providers;

namespace Minesweeper.GameServices.Utilities.Generators
{
    public class RandomPlayerSelector : IRandomPlayerSelector
    {
        private readonly IRandomNumberProvider _randomNumberProvider;

        public RandomPlayerSelector(IRandomNumberProvider randomNumberProvider)
        {
            _randomNumberProvider = randomNumberProvider ?? throw new ArgumentNullException(nameof(randomNumberProvider));
        }

        public Players SelectRandomPlayer()
        {
            return _randomNumberProvider.GetRandomNumber() % 2 == 0
                ? Players.Player1
                : Players.Player2;
        }
    }
}