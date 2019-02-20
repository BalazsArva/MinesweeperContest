using System;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Generators
{
    public class RandomPlayerSelector : IRandomPlayerSelector
    {
        public RandomPlayerSelector(IRandomNumberProvider randomNumberProvider)
        {
            _randomNumberProvider = randomNumberProvider ?? throw new ArgumentNullException(nameof(randomNumberProvider));
        }

        private readonly IRandomNumberProvider _randomNumberProvider;

        public Players SelectRandomPlayer()
        {
            return _randomNumberProvider.GetRandomNumber() % 2 == 0
                ? Players.Player1
                : Players.Player2;
        }
    }
}