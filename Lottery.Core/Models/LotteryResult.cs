namespace Lottery.Core.Models
{
    internal class LotteryResult
    {
        public decimal Revenue { get; init; }
        public decimal Profit { get; init; }
        public List<WinnerDisplayInfo> Winners { get; init; } = new();
    }

    internal class WinnerDisplayInfo
    {
        public int PlayerId { get; init; }
        public required string PlayerName { get; init; }

        public int WinningTicketsCount { get; init; }

        public decimal TotalAmountWon { get; init; }
    }
}
