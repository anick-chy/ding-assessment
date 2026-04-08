using Ding.Assessment.Core.Common;

namespace Ding.Assessment.Core.Domain;

public sealed class Account
{
    public Account(IEnumerable<Transaction> transactions)
    {
        _transactions = [.. transactions];

        foreach (Transaction transaction in _transactions)
        {
            ApplyTransaction(transaction);
        }
    }

    private readonly List<Transaction> _transactions;

    public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();
    public Amount CurrentBalance { get; private set; }

    public Transaction Deposit(Amount amount, DateTime date)
    {
        var transaction = Transaction.Create(TransactionType.Deposit, amount, date);
        _transactions.Add(transaction);
        ApplyTransaction(transaction);
        return transaction;
    }

    public Transaction Withdraw(Amount amount, DateTime date)
    {
        if (CurrentBalance < amount)
        {
            throw new InsufficientFundsException();
        }

        var transaction = Transaction.Create(TransactionType.Withdrawal, amount, date);
        _transactions.Add(transaction);
        ApplyTransaction(transaction);
        return transaction;
    }

    private void ApplyTransaction(Transaction transaction)
    {
        switch (transaction.Type)
        {
            case TransactionType.Deposit:
                CurrentBalance += transaction.Amount;
                break;
            case TransactionType.Withdrawal:
                if (CurrentBalance < transaction.Amount)
                {
                    throw new InvalidOperationException("Invalid transaction history: overdraft detected.");
                }
                CurrentBalance -= transaction.Amount;
                break;
        }
    }
}
