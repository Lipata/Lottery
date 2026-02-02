using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface ILotteryService
    {
        void InitializeGame();

        IEnumerable<Player> GetPlayers();

        LotteryResult ExecuteDraw();
    }
}
