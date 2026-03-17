using Huntly.Core.Shared.Entities;
using NUnit.Framework;

namespace Huntly.Core.Tests.Shared.Entities;

[TestFixture]
public class AuditableEntityTests
{
    private class TestAuditableEntity : AuditableEntity
    {
        public void CallUpdateTimestamp()
        {
            UpdateTimestamp();
        }
    }

    [Test]
    public void Instantiation_InitializesTimestamps()
    {
        var beforeCreation = DateTime.UtcNow.AddSeconds(-1);
        var entity = new TestAuditableEntity();
        var afterCreation = DateTime.UtcNow.AddSeconds(1);

        Assert.That(entity.CreatedAt, Is.GreaterThan(beforeCreation).And.LessThan(afterCreation));
        Assert.That(entity.UpdatedAt, Is.GreaterThan(beforeCreation).And.LessThan(afterCreation));
        Assert.That(entity.CreatedAt, Is.EqualTo(entity.UpdatedAt));
    }

    [Test]
    public void UpdateTimestamp_ChangesUpdatedAtButNotCreatedAt()
    {
        var entity = new TestAuditableEntity();
        var originalCreatedAt = entity.CreatedAt;
        var originalUpdatedAt = entity.UpdatedAt;

        Thread.Sleep(50); // Ensure a measurable time difference

        entity.CallUpdateTimestamp();

        Assert.That(entity.CreatedAt, Is.EqualTo(originalCreatedAt));
        Assert.That(entity.UpdatedAt, Is.GreaterThan(originalUpdatedAt));
    }
}
