using Ding.Assessment.Core.Application;
using Ding.Assessment.Core.Common;
using Ding.Assessment.Core.Domain;
using Moq;

namespace Ding.Assement.Test;

public class ApplicationTests
{
    private readonly Mock<IAccountRepository> _repoMock;
    private readonly Mock<IStatementPresentation> _presentationMock;
    private readonly AccountService _accountService;
    public ApplicationTests()
    {
        _repoMock = new Mock<IAccountRepository>();
        _presentationMock = new Mock<IStatementPresentation>();
        _accountService = new AccountService(_repoMock.Object, _presentationMock.Object);
    }

    [Fact]
    public void Deposit_ShouldPersistTransaction()
    {
        // Arrange
        _repoMock.Setup(r => r.GetTransactions()).Returns([]);

        // Act
        _accountService.Deposit(100m);

        // Assert
        _repoMock.Verify(d => d.AddTransaction(It.Is<Transaction>(t =>
            t.Amount == 100m &&
            t.Type == TransactionType.Deposit
        )), Times.Once);
    }

    [Fact]
    public void Withdraw_ShouldPersistTransaction()
    {
        List<Transaction> transactions = [
            Transaction.Create(TransactionType.Deposit, 200m)
        ];

        // Arrange
        _repoMock.Setup(r => r.GetTransactions()).Returns(transactions);

        // Act
        _accountService.Withdraw(100m);

        // Assert
        _repoMock.Verify(d => d.AddTransaction(It.Is<Transaction>(t =>
            t.Amount == 100m &&
            t.Type == TransactionType.Withdrawal
        )), Times.Once);
    }

    [Fact]
    public void Withdraw_ShouldThrow_WhenInsufficientFunds()
    {
        _repoMock.Setup(r => r.GetTransactions()).Returns([]);

        Assert.Throws<InsufficientFundsException>(() => _accountService.Withdraw(100m));

        _repoMock.Verify(r => r.AddTransaction(It.IsAny<Transaction>()), Times.Never);
    }

    [Fact]
    public void PrintStatement_ShouldPresentTransactions()
    {
        List<Transaction> transactions = [
            Transaction.Create(TransactionType.Deposit, 100m),
            Transaction.Create(TransactionType.Withdrawal, 30m),
            Transaction.Create(TransactionType.Deposit, 50m)
        ];
        // Arrange
        _repoMock.Setup(r => r.GetTransactions()).Returns(transactions);

        // Act
        _accountService.PrintStatement();

        // Assert
        _presentationMock.Verify(p => p.PrintStatement(It.Is<IEnumerable<TransactionDto>>(dtos =>
            dtos.Count() == 3 &&
            dtos.ElementAt(0).Type == TransactionType.Deposit && dtos.ElementAt(0).Amount == 100m && dtos.ElementAt(0).Balance == 100m &&
            dtos.ElementAt(1).Type == TransactionType.Withdrawal && dtos.ElementAt(1).Amount == 30m && dtos.ElementAt(1).Balance == 70m &&
            dtos.ElementAt(2).Type == TransactionType.Deposit && dtos.ElementAt(2).Amount == 50m && dtos.ElementAt(2).Balance == 120m
        )), Times.Once);
    }

    [Fact]
    public void PrintStatement_ShouldPresentTransactionsInOrder()
    {
        var t1 = Transaction.Create(TransactionType.Deposit, 100m, new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc));
        var t2 = Transaction.Create(TransactionType.Withdrawal, 30m, new DateTime(2026, 1, 3, 0, 0, 0, DateTimeKind.Utc));
        var t3 = Transaction.Create(TransactionType.Deposit, 50m, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        List<Transaction> transactions = [
            t1,
            t2,
            t3
        ];
        // Arrange
        _repoMock.Setup(r => r.GetTransactions()).Returns(transactions);

        // Act
        _accountService.PrintStatement();

        // Assert
        _presentationMock.Verify(p => p.PrintStatement(It.Is<IEnumerable<TransactionDto>>(dtos =>
            dtos.Count() == 3 &&
            dtos.ElementAt(0).CreatedAt == t3.CreatedAt &&
            dtos.ElementAt(1).CreatedAt == t1.CreatedAt &&
            dtos.ElementAt(2).CreatedAt == t2.CreatedAt &&

            dtos.ElementAt(0).Balance == 50m &&
            dtos.ElementAt(1).Balance == 150m &&
            dtos.ElementAt(2).Balance == 120m
        )), Times.Once);
    }
}
