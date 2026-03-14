using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;

namespace Aida.ParallelChange.Api.Tests.Unit.Application;

[TestFixture]
public sealed class CustomerContactApplicationErrorsTests
{
    [Test]
    public void CustomerContactAlreadyExistsException_sets_customer_id_and_message()
    {
        var exception = new CustomerContactAlreadyExistsException(42);

        exception.CustomerId.ShouldBe(42);
        exception.Message.ShouldBe("Customer contact '42' already exists.");
    }

    [Test]
    public void CustomerContactNotFoundException_sets_customer_id_and_message()
    {
        var exception = new CustomerContactNotFoundException(99);

        exception.CustomerId.ShouldBe(99);
        exception.Message.ShouldBe("Customer contact '99' was not found.");
    }
}
