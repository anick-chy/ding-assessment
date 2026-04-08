namespace Ding.Assessment.Core.Common;

public readonly record struct Amount
{
    public decimal Value { get; init; }

    public Amount(decimal value)
    {
        Value = value;
    }

    public static implicit operator decimal(Amount amount) => amount.Value;

    public static implicit operator Amount(decimal value) => new(value);
}
