using Lottery.Core.Interfaces;
using Lottery.Core.Models;

namespace Lottery.Core.Factories
{
    public class CpuPlayerFactory : BasePlayerFactory
    {
        private readonly IRandomGenerator _random;

        public CpuPlayerFactory(LotterySettings settings, IRandomGenerator random)
            : base(settings)
        {
            _random = random;
        }

        protected override string GenerateName(int id) => $"Player {id}";

        public override IEnumerable<Player> Create(int startId)
        {
            var playerCount = _random.Next(Settings.MinPlayers, Settings.MaxPlayers + 1);

            return Enumerable.Range(startId, playerCount)
                .Select(i => CreatePlayer(i));
        }
    }
}
