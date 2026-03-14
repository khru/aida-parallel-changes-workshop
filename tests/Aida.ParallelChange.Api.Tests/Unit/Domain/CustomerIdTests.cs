using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Tests.Unit.Domain;

[TestFixture]
public sealed class CustomerIdTests
{
    [TestCase(1)]
    [TestCase(42)]
    public void Constructor_accepts_positive_values(int value)
    {
        var customerId = new CustomerId(value);

        customerId.ShouldBe(new CustomerId(value));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public void Constructor_throws_when_value_is_not_positive(int value)
    {
        var exception = Should.Throw<ArgumentOutOfRangeException>(() => new CustomerId(value));

        exception.ParamName.ShouldBe("customerId");
        exception.Message.ShouldStartWith("Customer id must be greater than zero.");
    }
}
