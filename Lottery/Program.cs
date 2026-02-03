using Lottery;
using Lottery.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

try
{
    var configuration = ConfigurationHelper.BuildConfiguration();

    using var serviceProvider = ConfigurationHelper.BuildServiceProvider(configuration);
    var lotteryService = serviceProvider.GetRequiredService<ILotteryService>();
    var lotterySettings = ConfigurationHelper.GetLotterySettings(configuration);

    var playerName = ConsoleUI.DisplayWelcome(lotterySettings);
    lotteryService.InitializePlayers(playerName);

    var ticketCount = ConsoleUI.PromptForTicketCount(playerName);
    lotteryService.BuyTickets(ticketCount);

    var result = lotteryService.ExecuteDraw();

    var cpuPlayerCount = lotteryService.GetPlayers().Count(p => p.IsCPU);
    ConsoleUI.DisplayResults(result, cpuPlayerCount);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Configuration error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
