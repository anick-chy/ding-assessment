namespace Ding.Assessment.Core.Domain;

public sealed class InsufficientFundsException : Exception
{
    public InsufficientFundsException() : base("The account does not have sufficient funds for this operation.")
    {
    }
}

public sealed class InvalidAmountException : Exception
{
    public InvalidAmountException() : base("The amount provided is invalid. Amount must be greater than zero.")
    {
    }
}
