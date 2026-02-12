using Lottery.Core.Models;

namespace Lottery.Core.Factories
{
    public abstract class BasePlayerFactory
    {
        protected readonly LotterySettings Settings;

        protected BasePlayerFactory(LotterySettings settings)
        {
            Settings = settings;
        }

        protected abstract string GenerateName(int id);

        protected virtual Player CreatePlayer(int id, bool isCpu = true)
        {
            return new Player
            {
                Id = id,
                Name = GenerateName(id),
                Balance = Settings.InitialBalance,
                IsCPU = isCpu
            };
        }

        public abstract IEnumerable<Player> Create(int startId);
    }
}
