using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Persistence.Migrations;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
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
        var password = configuredPassword ?? "AidaTest_StableAa1!";

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
        ApplyMigrations(_connectionString);
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
        command.CommandText = "DELETE FROM dbo.customer_contacts;";
        await command.ExecuteNonQueryAsync();
    }

    private static void ApplyMigrations(string connectionString)
    {
        var services = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddSqlServer()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(CreateCustomerContactsTable).Assembly).For.Migrations())
            .BuildServiceProvider(false);

        using var scope = services.CreateScope();
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }

    [Test]
    public async Task Migration_setup_applies_customer_contacts_schema()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var tableCommand = connection.CreateCommand();
        tableCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts';";

        var versionOneCommand = connection.CreateCommand();
        versionOneCommand.CommandText = "SELECT COUNT(*) FROM dbo.VersionInfo WHERE Version = 202604010001;";

        var customerIdColumnCommand = connection.CreateCommand();
        customerIdColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'customer_id';";

        var contactNameColumnCommand = connection.CreateCommand();
        contactNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'contact_name';";

        var phoneColumnCommand = connection.CreateCommand();
        phoneColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone';";

        var emailColumnCommand = connection.CreateCommand();
        emailColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'email';";

        var tableCount = (int)(await tableCommand.ExecuteScalarAsync())!;
        var versionOneCount = (int)(await versionOneCommand.ExecuteScalarAsync())!;
        var customerIdColumnCount = (int)(await customerIdColumnCommand.ExecuteScalarAsync())!;
        var contactNameColumnCount = (int)(await contactNameColumnCommand.ExecuteScalarAsync())!;
        var phoneColumnCount = (int)(await phoneColumnCommand.ExecuteScalarAsync())!;
        var emailColumnCount = (int)(await emailColumnCommand.ExecuteScalarAsync())!;

        tableCount.ShouldBe(1);
        versionOneCount.ShouldBe(1);
        customerIdColumnCount.ShouldBe(1);
        contactNameColumnCount.ShouldBe(1);
        phoneColumnCount.ShouldBe(1);
        emailColumnCount.ShouldBe(1);
    }

    [Test]
    public async Task CreateAsync_creates_contact_when_customer_does_not_exist()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var contact = CustomerContactBuilder.Create()
            .WithCustomerId(41)
            .WithContactName("Ada Lovelace")
            .WithPhoneNumber("+44 123456789")
            .WithEmailAddress("ada.lovelace@example.com")
            .Build();

        await repository.CreateAsync(contact, CancellationToken.None);
        var result = await repository.FindByIdAsync(new CustomerId(41), CancellationToken.None);
        var found = result as FindCustomerContactResult.Found;

        found.ShouldNotBeNull();
        found.CustomerContact.ContactName.ShouldBe(new ContactName("Ada Lovelace"));
    }

    [Test]
    public async Task CreateAsync_returns_conflict_when_customer_already_exists()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var originalContact = CustomerContactBuilder.Create()
            .WithCustomerId(42)
            .WithContactName("Original Contact")
            .WithPhoneNumber("+44 123456789")
            .WithEmailAddress("original.contact@example.com")
            .Build();
        var conflictingContact = CustomerContactBuilder.Create()
            .WithCustomerId(42)
            .WithContactName("Conflicting Contact")
            .WithPhoneNumber("+44 123456789")
            .WithEmailAddress("conflicting.contact@example.com")
            .Build();

        await repository.CreateAsync(originalContact, CancellationToken.None);
        var exception = await Should.ThrowAsync<CustomerContactAlreadyExistsException>(() => repository.CreateAsync(conflictingContact, CancellationToken.None));

        exception.CustomerId.ShouldBe(42);
    }

    [Test]
    public async Task UpdateAsync_updates_contact_when_contact_exists()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var originalContact = CustomerContactBuilder.Create()
            .WithCustomerId(43)
            .WithContactName("Original Contact")
            .WithPhoneNumber("+44 123456789")
            .WithEmailAddress("original.contact@example.com")
            .Build();
        var updatedContact = CustomerContactBuilder.Create()
            .WithCustomerId(43)
            .WithContactName("Updated Contact")
            .WithPhoneNumber("+44 123456789")
            .WithEmailAddress("updated.contact@example.com")
            .Build();

        await repository.CreateAsync(originalContact, CancellationToken.None);
        await repository.UpdateAsync(updatedContact, CancellationToken.None);
        var result = await repository.FindByIdAsync(new CustomerId(43), CancellationToken.None);
        var found = result as FindCustomerContactResult.Found;

        found.ShouldNotBeNull();
        found.CustomerContact.ContactName.ShouldBe(new ContactName("Updated Contact"));
    }

    [Test]
    public async Task UpdateAsync_throws_not_found_when_contact_does_not_exist()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var contact = CustomerContactBuilder.Create()
            .WithCustomerId(44)
            .WithContactName("Non Existing")
            .WithPhoneNumber("+44 123456789")
            .WithEmailAddress("non.existing@example.com")
            .Build();

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
        notFound.CustomerId.ShouldBe(new CustomerId(999));
    }
}
