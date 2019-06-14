using System;
using System.Collections.Generic;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.GameEngine.ComputerPlayer
{
    public class RuleExecutor
    {
        private readonly IEnumerable<Rule> rules;

        public RuleExecutor(IEnumerable<Rule> rules)
        {
            this.rules = rules ?? throw new ArgumentNullException(nameof(rules));
        }

        public RuleExecutionResult Execute(Game game)
        {
            var random = new Random();

            return new RuleExecutionResult
            {
                OpenCol = random.Next(0, game.Columns),
                OpenRow = random.Next(0, game.Rows)
            };
        }
    }
}