using Lottery.Core.Models;

namespace Lottery;

public static class ConsoleUI
{
    public static string DisplayWelcome(LotterySettings settings)
    {
        Console.Write("Enter your name: ");
        var playerName = Console.ReadLine() ?? "Player 1";
        Console.WriteLine($@"

Welcome to the Bede Lottery, {playerName}!

* Your digital balance: {settings.InitialBalance:C}
* Ticket Price: {settings.TicketPrice:C}

");
        return playerName;
    }

    public static int PromptForTicketCount(string playerName)
    {
        Console.Write($"How many tickets do you want to buy, {playerName}? ");
        if (!int.TryParse(Console.ReadLine(), out var ticketCount) || ticketCount < 1 || ticketCount > 10)
        {
            Console.WriteLine("Invalid ticket count. Please enter a number between 1 and 10. Using default of 1.");
            return 1;
        }
        return ticketCount;
    }

    public static void DisplayResults(LotteryResult result, int cpuPlayerCount)
    {
        var grandPrizeDisplay = FormatGrandPrize(result.GrandPrizeWinners);
        var secondTierDisplay = FormatTierWinners(result.SecondTierWinners);
        var thirdTierDisplay = FormatTierWinners(result.ThirdTierWinners);

        Console.WriteLine($@"
{cpuPlayerCount} other CPU players also have purchased tickets.

Ticket Draw Results:

Grand Prize: {grandPrizeDisplay}

Second Tier: {secondTierDisplay}

Third Tier: {thirdTierDisplay}

Congratulations to the winners!

House Profit: {result.Profit:C}
");
    }

    private static string FormatGrandPrize(List<WinnerDisplayInfo> winners)
    {
        if (winners.Count == 0)
            return "No winner";

        var winner = winners.First();
        return $"{winner.Format()} wins {winner.TotalAmountWon:C}!";
    }

    private static string FormatTierWinners(List<WinnerDisplayInfo> winners)
    {
        if (winners.Count == 0)
            return "No winners";

        var prizePerTicket = winners.First().PrizePerTicket;
        var playersList = string.Join(", ", winners.Select(w => w.Format()));
        return $"Players {playersList} win {prizePerTicket:C} per winning ticket!";
    }
}
