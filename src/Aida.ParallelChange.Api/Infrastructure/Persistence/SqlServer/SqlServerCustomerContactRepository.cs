using Dapper;
using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Rows;
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

        return new FindCustomerContactResult.Found(CustomerContactBuilder.FromPrimitives(row.CustomerId, row.ContactName, row.Phone, row.Email));
    }

    public async Task CreateAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);

        try
        {
            var command = new CommandDefinition(
                SqlQueries.Create,
                new
                {
                    CustomerId = customerContact.CustomerId.Value,
                    ContactName = customerContact.ContactName.Value,
                    Phone = customerContact.Phone.Value,
                    Email = customerContact.Email.Value
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
        var command = new CommandDefinition(
            SqlQueries.Update,
            new
            {
                CustomerId = customerContact.CustomerId.Value,
                ContactName = customerContact.ContactName.Value,
                Phone = customerContact.Phone.Value,
                Email = customerContact.Email.Value
            },
            cancellationToken: cancellationToken);

        var affectedRows = await connection.ExecuteAsync(command);

        if (affectedRows != 1)
        {
            throw new CustomerContactNotFoundException(customerContact.CustomerId.Value);
        }
    }
}
