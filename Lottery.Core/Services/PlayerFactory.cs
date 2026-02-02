using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Services
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly LotterySettings _settings;
        private readonly IRandomGenerator _random;

        public PlayerFactory(LotterySettings settings, IRandomGenerator random)
        {
            _settings = settings;
            _random = random;
        }

        public Player CreateHumanPlayer(string name)
        {
            return new Player
            {
                Id = 1,
                Name = name,
                Balance = _settings.InitialBalance,
                IsCPU = false
            };
        }

        public IEnumerable<Player> CreateCpuPlayers(int startId)
        {
            var playerCount = _random.Next(_settings.MinPlayers, _settings.MaxPlayers + 1);

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
