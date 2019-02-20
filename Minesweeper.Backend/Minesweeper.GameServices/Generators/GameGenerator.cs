using System;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Generators
{
    public class GameGenerator : IGameGenerator
    {
        private readonly IGameTableGenerator _gameTableGenerator;
        private readonly IRandomPlayerSelector _randomPlayerSelector;
        private readonly IGuidProvider _guidProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public GameGenerator(IGameTableGenerator gameTableGenerator, IRandomPlayerSelector randomPlayerSelector, IGuidProvider guidProvider, IDateTimeProvider dateTimeProvider)
        {
            _gameTableGenerator = gameTableGenerator ?? throw new ArgumentNullException(nameof(gameTableGenerator));
            _randomPlayerSelector = randomPlayerSelector ?? throw new ArgumentNullException(nameof(randomPlayerSelector));
            _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public Game GenerateGame(int tableRows, int tableColumns, int mineCount)
        {
            var gameTable = _gameTableGenerator.GenerateGameTable(tableRows, tableColumns, mineCount);
            var starterPlayer = _randomPlayerSelector.SelectRandomPlayer();

            return new Game
            {
                Id = _guidProvider.GenerateGuidString(),
                EntryToken = _guidProvider.GenerateGuidString(),
                UtcDateTimeStarted = _dateTimeProvider.GetUtcDateTime(),
                GameTable = gameTable,
                StarterPlayer = starterPlayer
            };
        }
    }
}