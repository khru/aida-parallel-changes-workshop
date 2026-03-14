using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Aida.ParallelChange.Api.Tests.Integration;

[TestFixture]
[Category("NarrowIntegration")]
public sealed class SqlServerCustomerContactRepositoryTests
{
    private MsSqlContainer _sqlContainer = null!;
    private string _connectionString = string.Empty;

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var configuredPassword = Environment.GetEnvironmentVariable("AIDA_TEST_SQL_PASSWORD");
        var password = configuredPassword ?? $"AidaTest_{Guid.NewGuid():N}Aa1!";

        _sqlContainer = new MsSqlBuilder()
            .WithPassword(password)
            .Build();

        await _sqlContainer.StartAsync();

        var masterConnectionString = _sqlContainer.GetConnectionString();
        var databaseName = "AidaParallelChangeNarrowTests";

        await using (var connection = new SqlConnection(masterConnectionString))
        {
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"IF DB_ID('{databaseName}') IS NULL CREATE DATABASE [{databaseName}]";
            await command.ExecuteNonQueryAsync();
        }

        var connectionBuilder = new SqlConnectionStringBuilder(masterConnectionString)
        {
            InitialCatalog = databaseName
        };

        _connectionString = connectionBuilder.ConnectionString;
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        await _sqlContainer.DisposeAsync();
    }

    [SetUp]
    public async Task SetUp()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = """
            IF OBJECT_ID('dbo.customer_contacts', 'U') IS NOT NULL
                DROP TABLE dbo.customer_contacts;

            CREATE TABLE dbo.customer_contacts
            (
                customer_id INT NOT NULL PRIMARY KEY,
                contact_name NVARCHAR(200) NOT NULL,
                phone NVARCHAR(30) NOT NULL,
                email NVARCHAR(200) NOT NULL
            );
            """;
        await command.ExecuteNonQueryAsync();
    }

    [Test]
    public async Task CreateAsync_creates_contact_when_customer_does_not_exist()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var contact = CustomerContactBuilder.FromPrimitives(41, "Ada Lovelace", "+44 123456789", "ada.lovelace@example.com");

        await repository.CreateAsync(contact, CancellationToken.None);
        var result = await repository.FindByIdAsync(new CustomerId(41), CancellationToken.None);
        var found = result as FindCustomerContactResult.Found;

        found.ShouldNotBeNull();
        found.CustomerContact.ContactName.Value.ShouldBe("Ada Lovelace");
    }

    [Test]
    public async Task CreateAsync_returns_conflict_when_customer_already_exists()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var originalContact = CustomerContactBuilder.FromPrimitives(42, "Original Contact", "+44 123456789", "original.contact@example.com");
        var conflictingContact = CustomerContactBuilder.FromPrimitives(42, "Conflicting Contact", "+44 123456789", "conflicting.contact@example.com");

        await repository.CreateAsync(originalContact, CancellationToken.None);
        var exception = await Should.ThrowAsync<CustomerContactAlreadyExistsException>(() => repository.CreateAsync(conflictingContact, CancellationToken.None));

        exception.CustomerId.ShouldBe(42);
    }

    [Test]
    public async Task UpdateAsync_updates_contact_when_contact_exists()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var originalContact = CustomerContactBuilder.FromPrimitives(43, "Original Contact", "+44 123456789", "original.contact@example.com");
        var updatedContact = CustomerContactBuilder.FromPrimitives(43, "Updated Contact", "+44 123456789", "updated.contact@example.com");

        await repository.CreateAsync(originalContact, CancellationToken.None);
        await repository.UpdateAsync(updatedContact, CancellationToken.None);
        var result = await repository.FindByIdAsync(new CustomerId(43), CancellationToken.None);
        var found = result as FindCustomerContactResult.Found;

        found.ShouldNotBeNull();
        found.CustomerContact.ContactName.Value.ShouldBe("Updated Contact");
    }

    [Test]
    public async Task UpdateAsync_throws_not_found_when_contact_does_not_exist()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var contact = CustomerContactBuilder.FromPrimitives(44, "Non Existing", "+44 123456789", "non.existing@example.com");

        var exception = await Should.ThrowAsync<CustomerContactNotFoundException>(() => repository.UpdateAsync(contact, CancellationToken.None));

        exception.CustomerId.ShouldBe(44);
    }

    [Test]
    public async Task FindByIdAsync_returns_not_found_when_contact_does_not_exist()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);

        var result = await repository.FindByIdAsync(new CustomerId(999), CancellationToken.None);
        var notFound = result as FindCustomerContactResult.NotFound;

        notFound.ShouldNotBeNull();
        notFound.CustomerId.Value.ShouldBe(999);
    }
}
