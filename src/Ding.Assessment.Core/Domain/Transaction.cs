using Ding.Assessment.Core.Common;

namespace Ding.Assessment.Core.Domain;

public class Transaction
{
    public Guid Id { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public TransactionType Type { get; private set; }

    public Amount Amount { get; private set; }

    public static Transaction Create(TransactionType type, Amount amount, DateTime? createdAt = null)
    {
        if (amount.Value <= 0)
        {
            throw new InvalidAmountException();
        }
        return new Transaction
        {
            Id = Guid.NewGuid(),
            CreatedAt = createdAt ?? DateTime.UtcNow,
            Type = type,
            Amount = amount
        };
    }

    public static Transaction Restore(Guid id, TransactionType type, Amount amount, DateTime? createdAt = null)
    {
        return new Transaction
        {
            Id = id,
            CreatedAt = createdAt ?? DateTime.UtcNow,
            Type = type,
            Amount = amount
        };
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal
}
