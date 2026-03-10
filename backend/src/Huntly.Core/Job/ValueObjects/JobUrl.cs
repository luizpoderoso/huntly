namespace Huntly.Core.Job.ValueObjects;

public sealed class JobUrl
{
    public string Value { get; }

    public JobUrl(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Job URL cannot be empty.");
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Job URL must be a valid HTTP/HTTPS URL.");

        Value = value.Trim();
    }
}