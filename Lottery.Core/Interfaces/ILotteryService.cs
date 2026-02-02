using Lottery.Core.Models;

namespace Lottery.Core.Interfaces
{
    public interface ILotteryService
    {
        void InitializePlayers(string humanPlayerName);

        void BuyTickets(int humanTicketCount);

        IEnumerable<Player> GetPlayers();

        LotteryResult ExecuteDraw();
    }
}
