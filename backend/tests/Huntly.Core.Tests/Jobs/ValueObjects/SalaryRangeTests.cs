using Huntly.Core.Jobs.ValueObjects;
using NUnit.Framework;

namespace Huntly.Core.Tests.Jobs.ValueObjects;

[TestFixture]
public class SalaryRangeTests
{
    [Test]
    public void Create_ValidRange_CreatesSuccessfully()
    {
        var range = new SalaryRange(100000, 150000, "BRL");
        
        Assert.That(range.Min, Is.EqualTo(100000));
        Assert.That(range.Max, Is.EqualTo(150000));
        Assert.That(range.Currency, Is.EqualTo("BRL"));
    }

    [Test]
    public void Create_ValidRangeWithoutCurrency_DefaultsToUSD()
    {
        var range = new SalaryRange(80000, 90000);
        
        Assert.That(range.Currency, Is.EqualTo("USD"));
    }

    [Test]
    public void Create_NegativeMinimumSalary_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new SalaryRange(-1, 1000));
        Assert.That(ex.Message, Is.EqualTo("Minimum salary cannot be negative."));
    }

    [Test]
    public void Create_MaxLessThanMin_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new SalaryRange(150000, 100000));
        Assert.That(ex.Message, Is.EqualTo("Maximum salary cannot be less than minimum."));
    }

    [Test]
    public void Create_MaxEqualsMin_CreatesSuccessfully()
    {
        var range = new SalaryRange(100000, 100000);
        
        Assert.That(range.Min, Is.EqualTo(100000));
        Assert.That(range.Max, Is.EqualTo(100000));
    }
}
