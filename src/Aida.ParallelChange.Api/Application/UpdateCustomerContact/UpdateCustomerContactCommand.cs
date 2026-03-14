using Aida.ParallelChange.Api.Domain;

namespace Aida.ParallelChange.Api.Application.UpdateCustomerContact;

public readonly record struct UpdateCustomerContactCommand(CustomerContact CustomerContact);
