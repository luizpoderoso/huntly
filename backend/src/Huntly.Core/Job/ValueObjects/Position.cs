namespace Huntly.Core.Job.ValueObjects;

public sealed class Position
{
    public string Value { get; }

    public Position(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Position cannot be empty.");
        if (value.Length > 200)
            throw new ArgumentException("Position cannot exceed 200 characters.");

        Value = value.Trim();
    }
}