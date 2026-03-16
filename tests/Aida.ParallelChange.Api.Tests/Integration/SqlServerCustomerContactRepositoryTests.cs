using Aida.ParallelChange.Api.Application.CreateCustomerContact;
using Aida.ParallelChange.Api.Application.GetCustomerContact;
using Aida.ParallelChange.Api.Application.UpdateCustomerContact;
using Aida.ParallelChange.Api.Domain;
using Aida.ParallelChange.Api.Infrastructure.Persistence.Migrations;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Transition;
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

        var versionTwoCommand = connection.CreateCommand();
        versionTwoCommand.CommandText = "SELECT COUNT(*) FROM dbo.VersionInfo WHERE Version = 202604010002;";

        var customerIdColumnCommand = connection.CreateCommand();
        customerIdColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'customer_id';";

        var contactNameColumnCommand = connection.CreateCommand();
        contactNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'contact_name';";

        var phoneColumnCommand = connection.CreateCommand();
        phoneColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone';";

        var emailColumnCommand = connection.CreateCommand();
        emailColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'email';";

        var firstNameColumnCommand = connection.CreateCommand();
        firstNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'first_name';";

        var lastNameColumnCommand = connection.CreateCommand();
        lastNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'last_name';";

        var phoneCountryCodeColumnCommand = connection.CreateCommand();
        phoneCountryCodeColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone_country_code';";

        var phoneLocalNumberColumnCommand = connection.CreateCommand();
        phoneLocalNumberColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone_local_number';";

        var tableCount = (int)(await tableCommand.ExecuteScalarAsync())!;
        var versionOneCount = (int)(await versionOneCommand.ExecuteScalarAsync())!;
        var versionTwoCount = (int)(await versionTwoCommand.ExecuteScalarAsync())!;
        var customerIdColumnCount = (int)(await customerIdColumnCommand.ExecuteScalarAsync())!;
        var contactNameColumnCount = (int)(await contactNameColumnCommand.ExecuteScalarAsync())!;
        var phoneColumnCount = (int)(await phoneColumnCommand.ExecuteScalarAsync())!;
        var emailColumnCount = (int)(await emailColumnCommand.ExecuteScalarAsync())!;
        var firstNameColumnCount = (int)(await firstNameColumnCommand.ExecuteScalarAsync())!;
        var lastNameColumnCount = (int)(await lastNameColumnCommand.ExecuteScalarAsync())!;
        var phoneCountryCodeColumnCount = (int)(await phoneCountryCodeColumnCommand.ExecuteScalarAsync())!;
        var phoneLocalNumberColumnCount = (int)(await phoneLocalNumberColumnCommand.ExecuteScalarAsync())!;

        tableCount.ShouldBe(1);
        versionOneCount.ShouldBe(1);
        versionTwoCount.ShouldBe(1);
        customerIdColumnCount.ShouldBe(1);
        contactNameColumnCount.ShouldBe(1);
        phoneColumnCount.ShouldBe(1);
        emailColumnCount.ShouldBe(1);
        firstNameColumnCount.ShouldBe(1);
        lastNameColumnCount.ShouldBe(1);
        phoneCountryCodeColumnCount.ShouldBe(1);
        phoneLocalNumberColumnCount.ShouldBe(1);
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
    public async Task CreateAsync_writes_structured_columns_from_flat_contact_values()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var contact = CustomerContactBuilder.Create()
            .WithCustomerId(411)
            .WithContactName("Maria Garcia")
            .WithPhoneNumber("+34 600123123")
            .WithEmailAddress("maria.garcia@example.com")
            .Build();

        await repository.CreateAsync(contact, CancellationToken.None);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT first_name, last_name, phone_country_code, phone_local_number FROM dbo.customer_contacts WHERE customer_id = 411;";
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        reader.GetString(0).ShouldBe("Maria");
        reader.GetString(1).ShouldBe("Garcia");
        reader.GetString(2).ShouldBe("+34");
        reader.GetString(3).ShouldBe("600123123");
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
    public async Task UpdateAsync_updates_structured_columns_from_flat_contact_values()
    {
        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);
        var originalContact = CustomerContactBuilder.Create()
            .WithCustomerId(431)
            .WithContactName("Original Contact")
            .WithPhoneNumber("+34 600000001")
            .WithEmailAddress("original.contact@example.com")
            .Build();
        var updatedContact = CustomerContactBuilder.Create()
            .WithCustomerId(431)
            .WithContactName("Maria Garcia")
            .WithPhoneNumber("+34 600123123")
            .WithEmailAddress("updated.contact@example.com")
            .Build();

        await repository.CreateAsync(originalContact, CancellationToken.None);
        await repository.UpdateAsync(updatedContact, CancellationToken.None);

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT first_name, last_name, phone_country_code, phone_local_number FROM dbo.customer_contacts WHERE customer_id = 431;";
        await using var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();
        reader.GetString(0).ShouldBe("Maria");
        reader.GetString(1).ShouldBe("Garcia");
        reader.GetString(2).ShouldBe("+34");
        reader.GetString(3).ShouldBe("600123123");
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

    [Test]
    public async Task FindByIdAsync_composes_flat_projection_when_legacy_columns_are_blank()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var insertCommand = connection.CreateCommand();
        insertCommand.CommandText = """
            INSERT INTO dbo.customer_contacts
                (customer_id, contact_name, phone, email, first_name, last_name, phone_country_code, phone_local_number)
            VALUES
                (991, '', '', 'composed@example.com', 'Maria', 'Garcia', '+34', '600123123');
            """;
        await insertCommand.ExecuteNonQueryAsync();

        var connectionFactory = new DatabaseConnectionFactory(_connectionString);
        var repository = new SqlServerCustomerContactRepository(connectionFactory);

        var result = await repository.FindByIdAsync(new CustomerId(991), CancellationToken.None);
        var found = result as FindCustomerContactResult.Found;

        found.ShouldNotBeNull();
        found.CustomerContact.ContactName.ShouldBe(new ContactName("Maria Garcia"));
        found.CustomerContact.Phone.ShouldBe(new PhoneNumber("+34 600123123"));
    }

    [Test]
    public async Task BackfillBatchAsync_populates_structured_columns_for_pending_rows()
    {
        await using var seedConnection = new SqlConnection(_connectionString);
        await seedConnection.OpenAsync();
        var seedCommand = seedConnection.CreateCommand();
        seedCommand.CommandText = """
            INSERT INTO dbo.customer_contacts
                (customer_id, contact_name, phone, email)
            VALUES
                (2001, 'Maria Garcia', '+34 600123123', 'maria.garcia@example.com');
            """;
        await seedCommand.ExecuteNonQueryAsync();

        var processor = new StructuredCustomerContactBackfillProcessor(new DatabaseConnectionFactory(_connectionString));

        var result = await processor.BackfillBatchAsync(batchSize: 100, cancellationToken: CancellationToken.None);

        result.UpdatedRows.ShouldBe(1);
        result.RemainingRows.ShouldBe(0);

        var readCommand = seedConnection.CreateCommand();
        readCommand.CommandText = "SELECT first_name, last_name, phone_country_code, phone_local_number FROM dbo.customer_contacts WHERE customer_id = 2001;";
        await using var reader = await readCommand.ExecuteReaderAsync();

        await reader.ReadAsync();
        reader.GetString(0).ShouldBe("Maria");
        reader.GetString(1).ShouldBe("Garcia");
        reader.GetString(2).ShouldBe("+34");
        reader.GetString(3).ShouldBe("600123123");
    }

    [Test]
    public async Task BackfillBatchAsync_is_idempotent_after_rows_are_backfilled()
    {
        await using var seedConnection = new SqlConnection(_connectionString);
        await seedConnection.OpenAsync();
        var seedCommand = seedConnection.CreateCommand();
        seedCommand.CommandText = """
            INSERT INTO dbo.customer_contacts
                (customer_id, contact_name, phone, email)
            VALUES
                (2002, 'Ada Lovelace', '+44 123456789', 'ada.lovelace@example.com');
            """;
        await seedCommand.ExecuteNonQueryAsync();

        var processor = new StructuredCustomerContactBackfillProcessor(new DatabaseConnectionFactory(_connectionString));

        var firstRun = await processor.BackfillBatchAsync(batchSize: 100, cancellationToken: CancellationToken.None);
        var secondRun = await processor.BackfillBatchAsync(batchSize: 100, cancellationToken: CancellationToken.None);

        firstRun.UpdatedRows.ShouldBe(1);
        secondRun.UpdatedRows.ShouldBe(0);
        secondRun.RemainingRows.ShouldBe(0);
    }

    [Test]
    public async Task BackfillAllAsync_reports_completed_and_skipped_rows_for_mixed_dataset()
    {
        await using var seedConnection = new SqlConnection(_connectionString);
        await seedConnection.OpenAsync();
        var seedCommand = seedConnection.CreateCommand();
        seedCommand.CommandText = """
            INSERT INTO dbo.customer_contacts
                (customer_id, contact_name, phone, email, first_name, last_name, phone_country_code, phone_local_number)
            VALUES
                (2003, 'Grace Hopper', '+1 2025550101', 'grace.hopper@example.com', NULL, NULL, NULL, NULL),
                (2004, 'Margaret Hamilton', '+1 2025550102', 'margaret.hamilton@example.com', 'Margaret', 'Hamilton', '+1', '2025550102');
            """;
        await seedCommand.ExecuteNonQueryAsync();

        var processor = new StructuredCustomerContactBackfillProcessor(new DatabaseConnectionFactory(_connectionString));

        var result = await processor.BackfillAllAsync(batchSize: 1, cancellationToken: CancellationToken.None);

        result.TotalRows.ShouldBe(2);
        result.CompletedRows.ShouldBe(1);
        result.SkippedRows.ShouldBe(1);
    }
}
