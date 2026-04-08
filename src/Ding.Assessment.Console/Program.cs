using System.Globalization;
using Ding.Assessment.Core.Application;
using Ding.Assessment.Core.Common;
using Ding.Assessment.Core.Domain;
using Ding.Assessment.Core.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

string filePath = configuration["Storage:FilePath"] != null ? 
    Path.Combine(AppContext.BaseDirectory, configuration["Storage:FilePath"]!) : 
    throw new InvalidOperationException("File path is not configured. Please check appsettings.json.");

var services = new ServiceCollection();

services.AddSingleton<IConfiguration>(configuration);
services.AddTransient<IStatementPresentation, ConsolePresentation>();
services.AddTransient<IAccountRepository>(sp => new JsonAccountRepository(filePath));
services.AddTransient<IAccountService, AccountService>();

Console.WriteLine("Welcome! Please select an option:");
Console.WriteLine();

Console.WriteLine("1 for Deposit");
Console.WriteLine("2 for Withdraw");
Console.WriteLine("3 for Print Statement");
Console.WriteLine("q for exit");
Console.WriteLine();

using ServiceProvider serviceProvider = services.BuildServiceProvider();
using IServiceScope scope = serviceProvider.CreateScope();

IAccountService accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

string? input;

while ((input = Console.ReadLine()?.ToLower(CultureInfo.InvariantCulture)) != "q")
{
    switch (input)
    {
        case "1":
            Console.Write("Enter amount to deposit: ");
            HandleTransaction(amount => accountService.Deposit(amount));
            break;
        case "2":
            Console.Write("Enter amount to withdraw: ");
            HandleTransaction(amount => accountService.Withdraw(amount));
            break;
        case "3":
            accountService.PrintStatement();
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine("Select an option again or press q: ");
    Console.WriteLine();

}

static void HandleTransaction(Action<Amount> transactionAction)
{
    string? amntInput = Console.ReadLine();

    if (!decimal.TryParse(amntInput, out decimal amount))
    {
        Console.WriteLine("Invalid amount. Please enter a valid decimal number.");
        return;
    }
    try
    {
        transactionAction(amount);
        Console.WriteLine();
        Console.WriteLine("Transaction was successful");
    }
    catch (InsufficientFundsException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch(InvalidAmountException ex)
    {
        Console.WriteLine(ex.Message);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"An unexpected error occurred: {ex.Message}. Please try again");
    }
    
}
