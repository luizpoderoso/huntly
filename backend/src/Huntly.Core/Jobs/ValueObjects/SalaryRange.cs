namespace Huntly.Core.Jobs.ValueObjects;

public sealed class SalaryRange
{
    public decimal Min { get; }
    public decimal Max { get; }
    public string Currency { get; }

    public SalaryRange(decimal min, decimal max, string currency = "USD")
    {
        if (min < 0) throw new ArgumentException("Minimum salary cannot be negative.");
        if (max < min) throw new ArgumentException("Maximum salary cannot be less than minimum.");

        Min = min;
        Max = max;
        Currency = currency;
    }
}