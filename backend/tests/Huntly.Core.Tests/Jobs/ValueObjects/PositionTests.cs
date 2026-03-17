using Huntly.Core.Jobs.ValueObjects;
using NUnit.Framework;

namespace Huntly.Core.Tests.Jobs.ValueObjects;

[TestFixture]
public class PositionTests
{
    [Test]
    public void Create_ValidValue_CreatesSuccessfully()
    {
        var position = new Position("Software Engineer");
        Assert.That(position.Value, Is.EqualTo("Software Engineer"));
    }

    [Test]
    public void Create_ValueWithSurroundingSpaces_TrimsWhitespace()
    {
        var position = new Position("  Data Scientist  ");
        Assert.That(position.Value, Is.EqualTo("Data Scientist"));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public void Create_EmptyOrWhitespace_ThrowsArgumentException(string? value)
    {
        var ex = Assert.Throws<ArgumentException>(() => new Position(value));
        Assert.That(ex.Message, Is.EqualTo("Position cannot be empty."));
    }

    [Test]
    public void Create_ValueTooLong_ThrowsArgumentException()
    {
        var tooLong = new string('A', 201);
        var ex = Assert.Throws<ArgumentException>(() => new Position(tooLong));
        Assert.That(ex.Message, Is.EqualTo("Position cannot exceed 200 characters."));
    }
}
