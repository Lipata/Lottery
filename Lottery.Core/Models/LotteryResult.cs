namespace Lottery.Core.Models
{
    internal class LotteryResult
    {
        public decimal Revenue { get; init; }
        public decimal Profit { get; init; }
        public List<Player> Winners { get; init; } = new();
    }
}
