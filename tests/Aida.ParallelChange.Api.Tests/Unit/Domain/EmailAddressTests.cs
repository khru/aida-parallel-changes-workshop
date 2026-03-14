using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class EmailAddressTests
{
    [Test]
    public void Constructor_trims_email()
    {
        var email = new EmailAddress("  ada@example.com  ");

        email.Value.ShouldBe("ada@example.com");
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("invalid-email")]
    public void Constructor_throws_when_email_is_invalid(string value)
    {
        var exception = Should.Throw<ArgumentException>(() => new EmailAddress(value));

        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldStartWith("Email address is invalid.");
    }

    [Test]
    public void Constructor_throws_argument_exception_when_email_is_null()
    {
        var exception = Should.Throw<ArgumentException>(() => new EmailAddress(null!));

        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldStartWith("Email address is invalid.");
    }
}
