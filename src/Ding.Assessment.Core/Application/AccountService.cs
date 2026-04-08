using Ding.Assessment.Core.Common;
using Ding.Assessment.Core.Domain;

namespace Ding.Assessment.Core.Application;

public sealed class AccountService : IAccountService
{
    private static DateTime Now() => DateTime.UtcNow;
    private readonly IStatementPresentation statementPresentation;
    private readonly IAccountRepository accountRepository;

    public AccountService(IAccountRepository accountRepository, IStatementPresentation statementPresentation)
    {
        this.accountRepository = accountRepository;
        this.statementPresentation = statementPresentation;
    }

    public void Deposit(Amount amount)
    {
        Account account = LoadAccount();
        Transaction transaction = account.Deposit(amount, Now());
        accountRepository.AddTransaction(transaction);
    }

    public void PrintStatement()
    {
        IReadOnlyCollection<Transaction> transactions = accountRepository.GetTransactions();
        List<TransactionDto> transactionDtos = [];
        Amount balance = 0m;
        foreach (Transaction transaction in transactions.OrderBy(t => t.CreatedAt))
        {
            balance += transaction.Type == TransactionType.Deposit ? transaction.Amount : -transaction.Amount;
            transactionDtos.Add(new TransactionDto(
                transaction.Type,
                transaction.Amount,
                transaction.CreatedAt,
                balance
            ));
        }
        
        statementPresentation.PrintStatement(transactionDtos);
    }

    public void Withdraw(Amount amount)
    {
        Account account = LoadAccount();
        Transaction transaction = account.Withdraw(amount, Now());
        accountRepository.AddTransaction(transaction);
    }

    private Account LoadAccount()
    {
        IEnumerable<Transaction> transactions = accountRepository.GetTransactions().OrderBy(t => t.CreatedAt);
        return new Account(transactions);
    }
}
