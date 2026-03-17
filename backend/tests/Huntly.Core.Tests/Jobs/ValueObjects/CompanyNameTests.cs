using Huntly.Core.Jobs.ValueObjects;
using NUnit.Framework;

namespace Huntly.Core.Tests.Jobs.ValueObjects;

[TestFixture]
public class CompanyNameTests
{
    [Test]
    public void Create_ValidValue_CreatesSuccessfully()
    {
        var name = new CompanyName("Stripe");
        Assert.That(name.Value, Is.EqualTo("Stripe"));
    }

    [Test]
    public void Create_ValueWithSurroundingSpaces_TrimsWhitespace()
    {
        var name = new CompanyName("  Netflix  ");
        Assert.That(name.Value, Is.EqualTo("Netflix"));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void Create_EmptyOrWhitespace_ThrowsArgumentException(string? value)
    {
        var ex = Assert.Throws<ArgumentException>(() => new CompanyName(value));
        Assert.That(ex.Message, Is.EqualTo("Company name cannot be empty."));
    }

    [Test]
    public void Create_ValueTooLong_ThrowsArgumentException()
    {
        var tooLong = new string('A', 201);
        var ex = Assert.Throws<ArgumentException>(() => new CompanyName(tooLong));
        Assert.That(ex.Message, Is.EqualTo("Company name cannot exceed 200 characters."));
    }
}
