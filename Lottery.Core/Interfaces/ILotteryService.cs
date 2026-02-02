using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface ILotteryService
    {
        void InitializePlayers(string humanPlayerName);

        IEnumerable<Player> GetPlayers();

        LotteryResult ExecuteDraw();
    }
}
