namespace Aida.ParallelChange.Api.Domain;

public sealed class CustomerContact
{
    public CustomerId CustomerId { get; }
    public string ContactName { get; }
    public string Phone { get; }
    public EmailAddress Email { get; }

    public CustomerContact(CustomerId customerId, string contactName, string phone, EmailAddress email)
    {
        CustomerId = customerId;
        ContactName = contactName;
        Phone = phone;
        Email = email;
    }
}
