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
        Console.WriteLine($"\n{cpuPlayerCount} other CPU players also have purchased tickets.");
        Console.WriteLine("\nTicket Draw Results:\n");

        foreach (var tierResult in result.TierResults)
        {
            var display = FormatTierWinners(tierResult);
            Console.WriteLine($"{tierResult.TierName}: {display}\n");
        }

        Console.WriteLine("Congratulations to the winners!\n");
        Console.WriteLine($"House Profit: {result.Profit:C}\n");
    }

    private static string FormatTierWinners(TierResult tierResult)
    {
        if (tierResult.Winners.Count == 0)
            return "No winners";

        var winners = tierResult.Winners;
        var prizePerTicket = winners.First().PrizePerTicket;
        var playersList = string.Join(", ", winners.Select(w => w.Format()));

        if (winners.Count == 1 && winners.First().WinningTicketsCount == 1)
        {
            var winner = winners.First();
            return $"{winner.Format()} wins {winner.TotalAmountWon:C}!";
        }

        return $"Players {playersList} win {prizePerTicket:C} per winning ticket!";
    }
}
