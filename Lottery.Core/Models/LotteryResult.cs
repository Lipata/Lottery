namespace Lottery.Core.Models
{
    public class LotteryResult
    {
        public decimal Revenue { get; init; }
        public decimal Profit { get; init; }
        public List<WinnerDisplayInfo> Winners { get; init; } = new();
    }

    public class WinnerDisplayInfo
    {
        public int PlayerId { get; init; }
        public required string PlayerName { get; init; }

        public PrizeSettings? Tier { get; init; }

        public int WinningTicketsCount { get; init; }

        public decimal TotalAmountWon { get; init; }
    }
}
