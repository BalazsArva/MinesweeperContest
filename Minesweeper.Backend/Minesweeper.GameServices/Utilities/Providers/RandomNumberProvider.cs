using System;

namespace Minesweeper.GameServices.Utilities.Providers
{
    public class RandomNumberProvider : IRandomNumberProvider
    {
        private readonly Random _random = new Random();

        public int GetRandomNumber() => _random.Next();
    }
}