using Ding.Assessment.Core.Common;
using Ding.Assessment.Core.Domain;

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
            decimal amountValue = transaction.Type == TransactionType.Deposit ? transaction.Amount.Value : -transaction.Amount.Value;
            Console.WriteLine($"{transaction.CreatedAt:dd/MM/yyyy} || {amountValue} || {transaction.Balance.Value}");
        }
    }
}
