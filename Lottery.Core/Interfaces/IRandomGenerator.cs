namespace Lottery.Core.Interfaces
{
    public interface IRandomGenerator
    {
        int Next(int minValue, int maxValue);
    }
}
