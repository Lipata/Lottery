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
            return new LotteryResult
            {
                Revenue = 0m,
                Profit = 0m,
                Winners = new List<WinnerDisplayInfo>()
            };
        }
    }
}
