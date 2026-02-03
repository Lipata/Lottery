namespace Lottery.Core.Models
{
    public class LotteryResult
    {
        public decimal Revenue { get; init; }
        public decimal Profit { get; init; }
        public List<WinnerDisplayInfo> GrandPrizeWinners { get; init; } = new();
        public List<WinnerDisplayInfo> SecondTierWinners { get; init; } = new();
        public List<WinnerDisplayInfo> ThirdTierWinners { get; init; } = new();

        public IEnumerable<WinnerDisplayInfo> AllWinners =>
            GrandPrizeWinners.Concat(SecondTierWinners).Concat(ThirdTierWinners);
    }

    public class WinnerDisplayInfo
    {
        public int PlayerId { get; init; }
        public required string PlayerName { get; init; }
        public int WinningTicketsCount { get; init; }
        public decimal PrizePerTicket { get; init; }
        public decimal TotalAmountWon { get; init; }

        public string Format() => $"{PlayerName}({WinningTicketsCount})";
    }
}
