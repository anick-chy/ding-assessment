using Ding.Assessment.Core.Common;
using Ding.Assessment.Core.Domain;

namespace Ding.Assement.Test;

public class DomainTests
{
    private static DateTime Now() => DateTime.UtcNow;
    [Fact]
    public void Deposit_ShouldIncreaseBalance()
    {
        // Arrange
        Account account = CreateAccountWithBalance();

        Amount balance = account.CurrentBalance;

        // Act
        Amount amount = 100.50m;
        account.Deposit(amount, Now());

        // Assert
        Amount expected = balance + amount;
        Assert.Equal(account.CurrentBalance, expected);
    }

    [Fact]
    public void Withdraw_ShouldDecreaseBalance_WhenSufficientFunds()
    {
        // Arrange
        Account account = CreateAccountWithBalance();
        Amount balance = account.CurrentBalance;

        // Act
        Amount amount = 50.25m;
        account.Withdraw(amount, Now());

        // Assert
        Amount expected = balance - amount;
        Assert.Equal(account.CurrentBalance, expected);
    }

    [Fact]
    public void Withdraw_ShouldThrowException_WhenInsufficientFunds()
    {
        // Arrange
        Account account = CreateAccountWithBalance();

        // Act
        Amount amount = account.CurrentBalance + 100;

        // Assert
        Assert.Throws<InsufficientFundsException>(() => account.Withdraw(amount, Now()));
    }

    [Fact]
    public void MultipleTransactions_ShouldCalculateBalanceCorrectly()
    {
        // Arrange
        Account account = CreateEmptyAccount();

        // Act
        account.Deposit(200m, Now());
        account.Withdraw(50m, Now());
        account.Deposit(25m, Now());

        // Assert
        Amount expectedBalance = 200m - 50m + 25m;
        Assert.Equal(account.CurrentBalance, expectedBalance);
    }

    [Fact]
    public void AmountInvalid_ShouldThrowException()
    {
        // Arrange
        Account account = CreateAccountWithBalance();

        // Act & Assert
        Assert.Throws<InvalidAmountException>(() => account.Deposit(-100m, Now()));
        Assert.Throws<InvalidAmountException>(() => account.Withdraw(-50m, Now()));
        Assert.Throws<InvalidAmountException>(() => account.Withdraw(0m, Now()));
        Assert.Throws<InvalidAmountException>(() => account.Deposit(0m, Now()));
    }

    [Fact]
    public void Rehydrate_ShouldConstructAccountFromTransactions()
    {
        // Arrange
        List<Transaction> transactions = [
            Transaction.Create(TransactionType.Deposit, 100m),
            Transaction.Create(TransactionType.Withdrawal, 30m),
            Transaction.Create(TransactionType.Deposit, 50m)
        ];

        // Act
        var account = new Account(transactions);

        // Assert
        Amount expected = 100m - 30m + 50m;
        Assert.Equal(expected, account.CurrentBalance);
    }

    [Fact]
    public void Rehydrate_ShouldThrow_WhenInvalidTransactionHistory()
    {
        // Arrange
        List<Transaction> transactions = [
            Transaction.Create(TransactionType.Deposit, 30m),
            Transaction.Create(TransactionType.Withdrawal, 60m),
        ];

        
        // Assert
        Assert.Throws<InvalidOperationException>(() => new Account(transactions));
    }

    [Fact]
    public void Transactions_ShouldBeReadOnly()
    {
        // Arrange
        Account account = CreateAccountWithBalance();

        // Act
        IReadOnlyCollection<Transaction> transactions = account.Transactions;

        // Assert
        Assert.False(transactions is List<Transaction>);

        if (transactions is IList<Transaction> asIList)
        {
            Assert.Throws<NotSupportedException>(() => asIList.Add(Transaction.Create(TransactionType.Deposit, 1m, Now())));
        }
    }

    private static Account CreateAccountWithBalance()
    {
        List<Transaction> transactions = [];

        transactions.Add(Transaction.Create(TransactionType.Deposit, 100.50m));
        transactions.Add(Transaction.Create(TransactionType.Withdrawal, 50.25m));

        return new Account(transactions);
    }

    private static Account CreateEmptyAccount()
    {
        return new Account([]);
    }
}
