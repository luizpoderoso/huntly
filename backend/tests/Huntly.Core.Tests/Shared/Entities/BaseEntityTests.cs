using Huntly.Core.Shared.Entities;
using NUnit.Framework;

namespace Huntly.Core.Tests.Shared.Entities;

[TestFixture]
public class BaseEntityTests
{
    private class TestBaseEntity : BaseEntity
    {
    }

    [Test]
    public void Instantiation_AssignsNewGuid()
    {
        var entity = new TestBaseEntity();

        Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Instantiation_AssignsUniqueGuids()
    {
        var entity1 = new TestBaseEntity();
        var entity2 = new TestBaseEntity();

        Assert.That(entity1.Id, Is.Not.EqualTo(entity2.Id));
    }
}
