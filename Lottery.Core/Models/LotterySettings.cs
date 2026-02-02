namespace Lottery.Core.Models
{
    public class LotterySettings
    {
        public decimal TicketPrice { get; set; }
        public decimal InitialBalance { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int MinTicketsPerPlayer { get; set; }
        public int MaxTicketsPerPlayer { get; set; }
        public PrizeSettings Prizes { get; set; } = new();
    }

    public class PrizeSettings
    {
        public PrizeTier GrandPrize { get; set; } = new();
        public PrizeTier SecondTier { get; set; } = new();
        public PrizeTier ThirdTier { get; set; } = new();
    }

    public class PrizeTier
    {
        public decimal RevenuePercentage { get; set; }
        public int? FixedWinnerCount { get; set; }
        public decimal? WinnersPercentage { get; set; }
    }
}
