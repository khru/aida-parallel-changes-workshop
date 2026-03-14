using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class CustomerContactBuilderTests
{
    [Test]
    public void FromPrimitives_creates_contact_with_value_objects()
    {
        var contact = CustomerContactBuilder.FromPrimitives(7, "Grace Hopper", "+1 5550100", "grace.hopper@example.com");

        contact.CustomerId.Value.ShouldBe(7);
        contact.ContactName.Value.ShouldBe("Grace Hopper");
        contact.Phone.Value.ShouldBe("+1 5550100");
        contact.Email.Value.ShouldBe("grace.hopper@example.com");
    }

    [Test]
    public void FromPrimitives_throws_when_any_invariant_is_invalid()
    {
        var exception = Should.Throw<ArgumentException>(() =>
            CustomerContactBuilder.FromPrimitives(7, string.Empty, "+1 5550100", "grace.hopper@example.com"));

        exception.Message.ShouldStartWith("Contact name is required.");
    }
}
