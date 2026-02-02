using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface IPlayerFactory
    {
        Player CreateHumanPlayer(string name);
        IEnumerable<Player> CreateCpuPlayers(int startId);
    }
}
