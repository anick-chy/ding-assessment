using Ding.Assessment.Core.Common;

namespace Ding.Assessment.Core.Application;

public interface IAccountService
{
    void Deposit(Amount amount);
    void Withdraw(Amount amount);
    void PrintStatement();
}
