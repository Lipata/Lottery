namespace Lottery.Core.Models
{
    public class LotteryResult
    {
        public decimal Revenue { get; init; }
        public decimal Profit { get; init; }
        public List<TierResult> TierResults { get; init; } = new();

        public IEnumerable<WinnerDisplayInfo> AllWinners =>
            TierResults.SelectMany(t => t.Winners);
    }

    public class TierResult
    {
        public required string TierName { get; init; }
        public List<WinnerDisplayInfo> Winners { get; init; } = new();
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
