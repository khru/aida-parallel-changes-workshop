using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class PhoneNumberTests
{
    [Test]
    public void Constructor_trims_phone()
    {
        var phone = new PhoneNumber("  +44 123456789  ");

        phone.Value.ShouldBe("+44 123456789");
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_throws_when_phone_is_blank(string value)
    {
        var exception = Should.Throw<ArgumentException>(() => new PhoneNumber(value));

        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldStartWith("Phone number is required.");
    }

    [Test]
    public void Constructor_accepts_phone_with_exactly_30_characters()
    {
        var value = new string('1', PhoneNumber.MaximumLength);

        var phone = new PhoneNumber(value);

        phone.Value.ShouldBe(value);
    }

    [Test]
    public void Constructor_throws_when_phone_is_too_long()
    {
        var value = new string('1', PhoneNumber.MaximumLength + 1);

        var exception = Should.Throw<ArgumentException>(() => new PhoneNumber(value));

        exception.ParamName.ShouldBe("value");
        exception.Message.ShouldStartWith("Phone number is invalid.");
    }
}
