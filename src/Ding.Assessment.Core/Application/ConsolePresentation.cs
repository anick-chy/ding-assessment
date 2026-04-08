using Ding.Assessment.Core.Common;

namespace Ding.Assessment.Core.Application;

public sealed class ConsolePresentation : IStatementPresentation
{
    public void PrintStatement(IEnumerable<TransactionDto> transactions)
    {
        string header = "Date || Amount  || Balance";
        Console.WriteLine(header);

        IOrderedEnumerable<TransactionDto> orderedTransactions = transactions.OrderByDescending(t => t.CreatedAt);
        foreach (TransactionDto transaction in orderedTransactions)
        {
            Console.WriteLine($"{transaction.CreatedAt:dd/MM/yyyy} || {transaction.Amount.Value} || {transaction.Balance.Value}");
        }
    }
}
