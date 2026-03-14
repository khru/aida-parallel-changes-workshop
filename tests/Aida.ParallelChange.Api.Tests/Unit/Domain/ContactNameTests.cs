using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class ContactNameTests
{
    [Test]
    public void Constructor_trims_name()
    {
        var name = new ContactName("  Ada Lovelace  ");

        name.Value.ShouldBe("Ada Lovelace");
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_throws_when_name_is_blank(string value)
    {
        var exception = Should.Throw<ArgumentException>(() => new ContactName(value));

        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldStartWith("Contact name is required.");
    }
}
