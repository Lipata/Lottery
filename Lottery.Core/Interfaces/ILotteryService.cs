using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    internal interface ILotteryService
    {
        void InitializeGame();

        IEnumerable<Player> GetPlayers();

        LotteryResult ExecuteDraw();
    }
}
