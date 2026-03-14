namespace Aida.ParallelChange.Api.Domain;

public readonly record struct CustomerId
{
    private const int MinimumValue = 1;

    private const string ValueMustBeGreaterThanZeroMessage = "Customer id must be greater than zero.";

    public int Value { get; }

    public CustomerId(int value)
    {
        if (value < MinimumValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), ValueMustBeGreaterThanZeroMessage);
        }

        Value = value;
    }
}
