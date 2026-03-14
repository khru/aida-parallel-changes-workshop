namespace Aida.ParallelChange.Api.Domain;

public static class CustomerContactBuilder
{
    public static CustomerContact FromPrimitives(int customerId, string contactName, string phone, string email)
    {
        return new CustomerContact(
            new CustomerId(customerId),
            new ContactName(contactName),
            new PhoneNumber(phone),
            new EmailAddress(email));
    }
}
