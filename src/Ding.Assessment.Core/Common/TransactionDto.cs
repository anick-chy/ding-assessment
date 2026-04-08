using Ding.Assessment.Core.Domain;

namespace Ding.Assessment.Core.Common;

public sealed record TransactionDto(
    TransactionType Type,
    Amount Amount,
    DateTime CreatedAt,
    Amount Balance);
