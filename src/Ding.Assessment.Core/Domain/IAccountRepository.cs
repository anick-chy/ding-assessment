namespace Ding.Assessment.Core.Domain;

public interface IAccountRepository
{
    IReadOnlyCollection<Transaction> GetTransactions();
    void AddTransaction(Transaction transaction);
}
