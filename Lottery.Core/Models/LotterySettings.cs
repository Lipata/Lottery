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
        public List<PrizeTier> Prizes { get; set; } = new();
    }

    public class PrizeTier
    {
        public string Name { get; set; } = string.Empty;
        public decimal RevenuePercentage { get; set; }
        public int? FixedWinnerCount { get; set; }
        public decimal? WinnersPercentage { get; set; }
    }
}
