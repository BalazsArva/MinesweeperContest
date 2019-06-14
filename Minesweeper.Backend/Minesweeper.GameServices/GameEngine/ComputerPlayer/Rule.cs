namespace Minesweeper.GameServices.GameEngine.ComputerPlayer
{
    public class Rule
    {
        public RulePattern Pattern { get; set; }

        public RuleFitting Fit { get; set; }

        public RuleAction Action { get; set; }
    }
}