using Dapper;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Rows;
using Aida.ParallelChange.Api.Ports;

namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;

public sealed class SqlServerCustomerContactRepository : CustomerContactReader, CustomerContactWriter
{
    private readonly DatabaseConnectionFactory _connectionFactory;

    public SqlServerCustomerContactRepository(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CustomerContact?> FindByIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken) as IAsyncDisposable;
        var sqlConnection = (System.Data.IDbConnection)connection!;
        var row = await sqlConnection.QuerySingleOrDefaultAsync<CustomerContactV1Row>(SqlQueries.FindById, new { CustomerId = customerId.Value });

        if (row is null)
        {
            return null;
        }

        return new CustomerContact(
            new CustomerId(row.CustomerId),
            row.ContactName,
            row.Phone,
            new EmailAddress(row.Email));
    }

    public async Task UpsertAsync(CustomerContact customerContact, CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken) as IAsyncDisposable;
        var sqlConnection = (System.Data.IDbConnection)connection!;
        await sqlConnection.ExecuteAsync(SqlQueries.Upsert, new
        {
            CustomerId = customerContact.CustomerId.Value,
            ContactName = customerContact.ContactName,
            Phone = customerContact.Phone,
            Email = customerContact.Email.Value
        });
    }
}
