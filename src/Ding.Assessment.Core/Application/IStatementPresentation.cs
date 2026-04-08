using Ding.Assessment.Core.Common;

namespace Ding.Assessment.Core.Application;

public interface IStatementPresentation
{
    void PrintStatement(IEnumerable<TransactionDto> transactions);
}
