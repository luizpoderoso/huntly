using Huntly.Core.Auth.Entities;
using NUnit.Framework;

namespace Huntly.Core.Tests.Auth.Entities;

[TestFixture]
public class UserTests
{
    [Test]
    public void Create_ValidParameters_InstantiatesProperly()
    {
        var user = User.Create("John Doe", "john@email.com", "johndoe");

        Assert.That(user.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(user.FullName, Is.EqualTo("John Doe"));
        Assert.That(user.Email, Is.EqualTo("john@email.com"));
        Assert.That(user.UserName, Is.EqualTo("johndoe"));
    }

    [Test]
    public void ChangeFullName_ValidName_UpdatesNameAndTimestamp()
    {
        var user = User.Create("John Doe", "john@email.com", "johndoe");
        var originalTimestamp = user.UpdatedAt;
        
        Thread.Sleep(50); // Ensuring timestamp gap

        user.ChangeFullName("Johnny Doe");

        Assert.That(user.FullName, Is.EqualTo("Johnny Doe"));
        Assert.That(user.UpdatedAt, Is.GreaterThan(originalTimestamp));
    }
}
