using Lottery.Core.Interfaces;

namespace Lottery.Core.Services
{
    public class RandomGenerator : IRandomGenerator
    {
        public int Next(int minValue, int maxValue)
        {
            return Random.Shared.Next(minValue, maxValue);
        }
    }
}
