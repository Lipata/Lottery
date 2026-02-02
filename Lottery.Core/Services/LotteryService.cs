using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class LotteryService : ILotteryService
    {
        private readonly List<Player> _players = new();
        private readonly LotterySettings _settings;
        private readonly IRandomGenerator _random;

        public LotteryService(LotterySettings settings, IRandomGenerator random)
        {
            _settings = settings;
            _random = random;
        }

        public void InitializePlayers(string humanPlayerName)
        {
            _players.Clear();

            _players.Add(CreateHumanPlayer(humanPlayerName));
            _players.AddRange(GenerateCpuPlayers());
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

                BuyTicketsForPlayer(player, ticketCount);
            }
        }

        public LotteryResult ExecuteDraw()
        {
            return new LotteryResult
            {
                Revenue = 0m,
                Profit = 0m,
                Winners = new List<WinnerDisplayInfo>()
            };
        }

        private void BuyTicketsForPlayer(Player player, int ticketCount)
        {
            for (int i = 0; i < ticketCount; i++)
            {
                if (player.Balance < _settings.TicketPrice)
                    break;

                player.Balance -= _settings.TicketPrice;
                player.Tickets.Add(new Ticket { PlayerId = player.Id });
            }
        }

        private Player CreateHumanPlayer(string name)
        {
            return new Player
            {
                Id = 1,
                Name = name,
                Balance = _settings.InitialBalance,
                IsCPU = false
            };
        }

        private IEnumerable<Player> GenerateCpuPlayers()
        {
            var playerCount = _random.Next(_settings.MinPlayers, _settings.MaxPlayers + 1);
            var startId = _players.Count + 1;

            return Enumerable.Range(startId, playerCount)
                .Select(i => new Player
                {
                    Id = i,
                    Name = $"Player {i}",
                    Balance = _settings.InitialBalance
                });
        }
    }
}
