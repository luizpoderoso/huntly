namespace Huntly.Core.Job.ValueObjects;

public sealed class Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty.");
        if (!value.Contains('@') || value.Length > 320)
            throw new ArgumentException("Email is not valid.");

        Value = value.Trim().ToLowerInvariant();
    }
}