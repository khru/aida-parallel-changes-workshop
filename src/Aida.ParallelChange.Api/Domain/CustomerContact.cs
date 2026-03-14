namespace Aida.ParallelChange.Api.Domain;

public sealed class CustomerContact
{
    public CustomerId CustomerId { get; }
    public ContactName ContactName { get; }
    public PhoneNumber Phone { get; }
    public EmailAddress Email { get; }

    public CustomerContact(CustomerId customerId, ContactName contactName, PhoneNumber phone, EmailAddress email)
    {
        CustomerId = customerId;
        ContactName = contactName;
        Phone = phone;
        Email = email;
    }
}
