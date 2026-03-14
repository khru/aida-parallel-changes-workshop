using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class PhoneNumberTests
{
    [Test]
    public void Constructor_trims_phone()
    {
        var phone = new PhoneNumber("  +44 123456789  ");

        phone.ShouldBe(new PhoneNumber("+44 123456789"));
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Constructor_throws_when_phone_is_blank(string phoneNumber)
    {
        var exception = Should.Throw<ArgumentException>(() => new PhoneNumber(phoneNumber));

        exception.ParamName.ShouldBe("phoneNumber");
        exception.Message.ShouldStartWith("Phone number is required.");
    }

    [Test]
    public void Constructor_accepts_phone_with_exactly_30_characters()
    {
        var phoneNumber = new string('1', 30);

        var phone = new PhoneNumber(phoneNumber);

        phone.ShouldBe(new PhoneNumber(phoneNumber));
    }

    [Test]
    public void Constructor_throws_when_phone_is_too_long()
    {
        var phoneNumber = new string('1', 31);

        var exception = Should.Throw<ArgumentException>(() => new PhoneNumber(phoneNumber));

        exception.ParamName.ShouldBe("phoneNumber");
        exception.Message.ShouldStartWith("Phone number is invalid.");
    }
}
