using Lottery;
using Lottery.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

try
{
    var configuration = ConfigurationHelper.BuildConfiguration();
    var lotterySettings = ConfigurationHelper.GetLotterySettings(configuration);

    using var serviceProvider = new ServiceCollection()
        .AddLotteryServices(lotterySettings)
        .BuildServiceProvider();

    var lotteryService = serviceProvider.GetRequiredService<ILotteryService>();

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
