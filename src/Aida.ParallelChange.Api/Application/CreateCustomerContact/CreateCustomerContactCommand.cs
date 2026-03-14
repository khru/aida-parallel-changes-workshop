using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Application.CreateCustomerContact;

public readonly record struct CreateCustomerContactCommand(CustomerContact CustomerContact);
