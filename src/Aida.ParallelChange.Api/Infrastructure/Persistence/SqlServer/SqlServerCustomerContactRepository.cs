using Dapper;
using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Rows;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Transition;
using Microsoft.Data.SqlClient;

namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;

public sealed class SqlServerCustomerContactRepository : CustomerContactReader, CustomerContactCreator, CustomerContactUpdater
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public SqlServerCustomerContactRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<FindCustomerContactResult> FindByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(SqlQueries.FindById, new { CustomerId = customerId.Value }, cancellationToken: cancellationToken);
        var row = await connection.QuerySingleOrDefaultAsync<CustomerContactV1Row>(command);

        if (row is null)
        {
            return new FindCustomerContactResult.NotFound(customerId);
        }

        var contactName = ResolveContactName(row);
        var phoneNumber = ResolvePhoneNumber(row);

        return new FindCustomerContactResult.Found(
            CustomerContactBuilder.Create()
                .WithCustomerId(row.CustomerId)
                .WithContactName(contactName)
                .WithPhoneNumber(phoneNumber)
                .WithEmailAddress(row.Email)
                .Build());
    }

    public async Task CreateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        try
        {
            var nameParts = CustomerContactTransitionPolicy.SplitName(customerContact.ContactName.Value);
            var phoneParts = CustomerContactTransitionPolicy.SplitPhone(customerContact.Phone.Value);

            var command = new CommandDefinition(
                SqlQueries.Create,
                new
                {
                    CustomerId = customerContact.CustomerId.Value,
                    ContactName = customerContact.ContactName.Value,
                    Phone = customerContact.Phone.Value,
                    Email = customerContact.Email.Value,
                    FirstName = nameParts.FirstName,
                    LastName = nameParts.LastName,
                    PhoneCountryCode = phoneParts.CountryCode,
                    PhoneLocalNumber = phoneParts.LocalNumber
                },
                cancellationToken: cancellationToken);

            await connection.ExecuteAsync(command);
        }
        catch (SqlException exception) when (exception.Number is 2601 or 2627)
        {
            throw new CustomerContactAlreadyExistsException(customerContact.CustomerId.Value);
        }
    }

    public async Task UpdateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var nameParts = CustomerContactTransitionPolicy.SplitName(customerContact.ContactName.Value);
        var phoneParts = CustomerContactTransitionPolicy.SplitPhone(customerContact.Phone.Value);
        var command = new CommandDefinition(
            SqlQueries.Update,
            new
            {
                CustomerId = customerContact.CustomerId.Value,
                ContactName = customerContact.ContactName.Value,
                Phone = customerContact.Phone.Value,
                Email = customerContact.Email.Value,
                FirstName = nameParts.FirstName,
                LastName = nameParts.LastName,
                PhoneCountryCode = phoneParts.CountryCode,
                PhoneLocalNumber = phoneParts.LocalNumber
            },
            cancellationToken: cancellationToken);

        var affectedRows = await connection.ExecuteAsync(command);

        if (affectedRows != 1)
        {
            throw new CustomerContactNotFoundException(customerContact.CustomerId.Value);
        }
    }

    private static string ResolveContactName(CustomerContactV1Row row)
    {
        if (!string.IsNullOrWhiteSpace(row.ContactName))
        {
            return row.ContactName!;
        }

        var composedName = CustomerContactTransitionPolicy.ComposeName(row.FirstName ?? string.Empty, row.LastName ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(composedName))
        {
            return composedName;
        }

        throw new InvalidOperationException($"Customer contact '{row.CustomerId}' has no resolvable contact name.");
    }

    private static string ResolvePhoneNumber(CustomerContactV1Row row)
    {
        if (!string.IsNullOrWhiteSpace(row.Phone))
        {
            return row.Phone!;
        }

        var composedPhone = CustomerContactTransitionPolicy.ComposePhone(row.PhoneCountryCode ?? string.Empty, row.PhoneLocalNumber ?? string.Empty);
        if (!string.IsNullOrWhiteSpace(composedPhone))
        {
            return composedPhone;
        }

        throw new InvalidOperationException($"Customer contact '{row.CustomerId}' has no resolvable phone number.");
    }
}
