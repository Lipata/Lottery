using Lottery.Core.Interfaces;
using Lottery.Core.Models;
using Lottery.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

try
{
    var configuration = BuildConfiguration();

    using var serviceProvider = BuildServiceProvider(configuration);
    var lotteryService = serviceProvider.GetRequiredService<ILotteryService>();
    var lotterySettings = GetLotterySettings(configuration);

    Console.Write("Enter your name: ");
    var playerName = Console.ReadLine() ?? "Player 1";
    Console.WriteLine($@"

Welcome to the Bede Lottery, {playerName}!

* Your digital balance: {lotterySettings.InitialBalance:C}
* Ticket Price: {lotterySettings.TicketPrice:C}

");



    lotteryService.InitializePlayers(playerName);

    Console.Write($"How many tickets do you want to buy, {playerName}? ");
    if (!int.TryParse(Console.ReadLine(), out var ticketCount) || ticketCount < 1 || ticketCount > 10)
    {
        Console.WriteLine("Invalid ticket count. Please enter a number between 1 and 10. Using default of 1.");
        ticketCount = 1;
    }
    lotteryService.BuyTickets(ticketCount);

    var result = lotteryService.ExecuteDraw();

    var cpuPlayerCount = lotteryService.GetPlayers().Count(p => p.IsCPU);
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
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Configuration error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

static IConfiguration BuildConfiguration()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();
}

static LotterySettings GetLotterySettings(IConfiguration configuration) { 
    return configuration.GetSection("LotterySettings").Get<LotterySettings>()
        ?? throw new InvalidOperationException("LotterySettings configuration is missing");
}
static ServiceProvider BuildServiceProvider(IConfiguration configuration)
{
    return new ServiceCollection()
        .AddSingleton(GetLotterySettings(configuration))
        .AddSingleton<IRandomGenerator, RandomGenerator>()
        .AddTransient<IPlayerFactory, PlayerFactory>()
        .AddTransient<ITicketService, TicketService>()
        .AddTransient<ILotteryService, LotteryService>()
        .BuildServiceProvider();
}

static string FormatGrandPrize(List<WinnerDisplayInfo> winners)
{
    if (winners.Count == 0)
        return "No winner";

    var winner = winners.First();
    return $"{winner.Format()} wins {winner.TotalAmountWon:C}!";
}

static string FormatTierWinners(List<WinnerDisplayInfo> winners)
{
    if (winners.Count == 0)
        return "No winners";

    var prizePerTicket = winners.First().PrizePerTicket;
    var playersList = string.Join(", ", winners.Select(w => w.Format()));
    return $"Players {playersList} win {prizePerTicket:C} per winning ticket!";
}
