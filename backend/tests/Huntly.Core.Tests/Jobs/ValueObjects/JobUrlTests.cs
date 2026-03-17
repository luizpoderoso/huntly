using Huntly.Core.Jobs.ValueObjects;
using NUnit.Framework;

namespace Huntly.Core.Tests.Jobs.ValueObjects;

[TestFixture]
public class JobUrlTests
{
    [Test]
    public void Create_ValidUrl_CreatesSuccessfully()
    {
        var url = new JobUrl("https://careers.google.com");
        Assert.That(url.Value, Is.EqualTo("https://careers.google.com"));
    }

    [Test]
    public void Create_ValidHttpUrl_CreatesSuccessfully()
    {
        var url = new JobUrl("http://old-site.com");
        Assert.That(url.Value, Is.EqualTo("http://old-site.com"));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void Create_EmptyOrWhitespace_ThrowsArgumentException(string? value)
    {
        var ex = Assert.Throws<ArgumentException>(() => new JobUrl(value));
        Assert.That(ex.Message, Is.EqualTo("Job URL cannot be empty."));
    }

    [TestCase("not-a-url")]
    [TestCase("ftp://careers.site")]
    [TestCase("www.google.com")] // Missing scheme
    public void Create_InvalidFormat_ThrowsArgumentException(string value)
    {
        var ex = Assert.Throws<ArgumentException>(() => new JobUrl(value));
        Assert.That(ex.Message, Is.EqualTo("Job URL must be a valid HTTP/HTTPS URL."));
    }
}
