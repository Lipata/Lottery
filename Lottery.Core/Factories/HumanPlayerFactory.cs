using Lottery.Core.Models;

namespace Lottery.Core.Factories
{
    public class HumanPlayerFactory : BasePlayerFactory
    {
        private readonly string _name;

        public HumanPlayerFactory(LotterySettings settings, string name) : base(settings)
        {
            _name = name;
        }

        protected override string GenerateName(int id) => _name;

        public override IEnumerable<Player> Create(int startId)
        {
            return [CreatePlayer(startId, isCpu: false)];
        }
    }
}
