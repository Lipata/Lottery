using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class LotteryService : ILotteryService
    {
        private readonly List<Player> _players = new();
        private readonly LotterySettings _settings;
        private readonly IRandomGenerator _random;
        private readonly IPlayerFactory _playerFactory;
        private readonly ITicketService _ticketService;

        public LotteryService(
            LotterySettings settings,
            IRandomGenerator random,
            IPlayerFactory playerFactory,
            ITicketService ticketService)
        {
            _settings = settings;
            _random = random;
            _playerFactory = playerFactory;
            _ticketService = ticketService;
        }

        public void InitializePlayers(string humanPlayerName)
        {
            _players.Clear();

            _players.Add(_playerFactory.CreateHumanPlayer(humanPlayerName));
            _players.AddRange(_playerFactory.CreateCpuPlayers(_players.Count + 1));
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _players;
        }

        public void BuyTickets(int humanTicketCount)
        {
            foreach (var player in _players)
            {
                var ticketCount = player.IsCPU
                    ? _random.Next(_settings.MinTicketsPerPlayer, _settings.MaxTicketsPerPlayer + 1)
                    : humanTicketCount;

                _ticketService.BuyTickets(player, ticketCount);
            }
        }

        public LotteryResult ExecuteDraw()
        {
            var revenue = CalculateRevenue();
            var allTickets = GetAllTickets();
            var winners = new List<WinnerDisplayInfo>();

            var grandPrizeWinners = PickWinnersForTier(_settings.Prizes.GrandPrize, revenue, allTickets);
            var secondTierWinners = PickWinnersForTier(_settings.Prizes.SecondTier, revenue, allTickets);
            var thirdTierWinners = PickWinnersForTier(_settings.Prizes.ThirdTier, revenue, allTickets);

            winners.AddRange(grandPrizeWinners);
            winners.AddRange(secondTierWinners);
            winners.AddRange(thirdTierWinners);

            var totalPrizesPaid = winners.Sum(w => w.TotalAmountWon);
            var profit = CalculateProfit(revenue, totalPrizesPaid);

            return new LotteryResult
            {
                Revenue = revenue,
                Profit = profit,
                Winners = winners
            };
        }

        private decimal CalculateRevenue()
        {
            var totalTickets = _players.Sum(p => p.Tickets.Count);
            return totalTickets * _settings.TicketPrice;
        }

        private List<Ticket> GetAllTickets()
        {
            return _players.SelectMany(p => p.Tickets).ToList();
        }

        private List<WinnerDisplayInfo> PickWinnersForTier(PrizeTier tier, decimal revenue, List<Ticket> allTickets)
        {
            if (allTickets.Count == 0)
                return new List<WinnerDisplayInfo>();

            var prizePool = revenue * tier.RevenuePercentage;
            var winnerCount = GetWinnerCount(tier, _players.Count);

            if (winnerCount == 0)
                return new List<WinnerDisplayInfo>();

            var winningTickets = PickRandomTickets(allTickets, winnerCount);
            var prizePerWinner = prizePool / winnerCount;

            return winningTickets
                .GroupBy(t => t.PlayerId)
                .Select(g => CreateWinnerInfo(g.Key, tier, g.Count(), prizePerWinner))
                .ToList();
        }

        private int GetWinnerCount(PrizeTier tier, int playerCount)
        {
            if (tier.FixedWinnerCount.HasValue)
                return tier.FixedWinnerCount.Value;

            if (tier.WinnersPercentage.HasValue)
                return Math.Max(1, (int)(playerCount * tier.WinnersPercentage.Value));

            return 0;
        }

        private List<Ticket> PickRandomTickets(List<Ticket> tickets, int count)
        {
            var selected = new List<Ticket>();
            var available = tickets.ToList();

            for (int i = 0; i < count && available.Count > 0; i++)
            {
                var index = _random.Next(0, available.Count);
                selected.Add(available[index]);
                available.RemoveAt(index);
            }

            return selected;
        }

        private WinnerDisplayInfo CreateWinnerInfo(int playerId, PrizeTier tier, int winningTicketsCount, decimal prizePerTicket)
        {
            var player = _players.First(p => p.Id == playerId);
            return new WinnerDisplayInfo
            {
                PlayerId = playerId,
                PlayerName = player.Name,
                WinningTicketsCount = winningTicketsCount,
                TotalAmountWon = winningTicketsCount * prizePerTicket
            };
        }

        private decimal CalculateProfit(decimal revenue, decimal totalPrizesPaid)
        {
            return revenue - totalPrizesPaid;
        }
    }
}
