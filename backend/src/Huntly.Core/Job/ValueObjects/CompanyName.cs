namespace Huntly.Core.Job.ValueObjects;

public sealed class CompanyName
{
    public string Value { get; }

    public CompanyName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Company name cannot be empty.");
        if (value.Length > 200)
            throw new ArgumentException("Company name cannot exceed 200 characters.");

        Value = value.Trim();
    }

    public override string ToString() => Value;
}