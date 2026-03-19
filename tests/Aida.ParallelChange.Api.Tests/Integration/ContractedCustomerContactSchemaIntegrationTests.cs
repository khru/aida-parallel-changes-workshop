using Aida.ParallelChange.Api.Infrastructure.Persistence.Migrations;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.MsSql;

namespace Aida.ParallelChange.Api.Tests.Integration;

[TestFixture]
[Category("NarrowIntegration")]
public sealed class ContractedCustomerContactSchemaIntegrationTests
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
        var databaseName = "AidaParallelChangeContractTests";

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

    [Test]
    public async Task Migration_setup_applies_contracted_customer_contacts_schema_without_legacy_flat_columns()
    {
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var versionThreeCommand = connection.CreateCommand();
        versionThreeCommand.CommandText = "SELECT COUNT(*) FROM dbo.VersionInfo WHERE Version = 202604010003;";

        var contactNameColumnCommand = connection.CreateCommand();
        contactNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'contact_name';";

        var phoneColumnCommand = connection.CreateCommand();
        phoneColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone';";

        var firstNameColumnCommand = connection.CreateCommand();
        firstNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'first_name';";

        var lastNameColumnCommand = connection.CreateCommand();
        lastNameColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'last_name';";

        var phoneCountryCodeColumnCommand = connection.CreateCommand();
        phoneCountryCodeColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone_country_code';";

        var phoneLocalNumberColumnCommand = connection.CreateCommand();
        phoneLocalNumberColumnCommand.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'customer_contacts' AND COLUMN_NAME = 'phone_local_number';";

        var versionThreeCount = (int)(await versionThreeCommand.ExecuteScalarAsync())!;
        var contactNameColumnCount = (int)(await contactNameColumnCommand.ExecuteScalarAsync())!;
        var phoneColumnCount = (int)(await phoneColumnCommand.ExecuteScalarAsync())!;
        var firstNameColumnCount = (int)(await firstNameColumnCommand.ExecuteScalarAsync())!;
        var lastNameColumnCount = (int)(await lastNameColumnCommand.ExecuteScalarAsync())!;
        var phoneCountryCodeColumnCount = (int)(await phoneCountryCodeColumnCommand.ExecuteScalarAsync())!;
        var phoneLocalNumberColumnCount = (int)(await phoneLocalNumberColumnCommand.ExecuteScalarAsync())!;

        versionThreeCount.ShouldBe(1);
        contactNameColumnCount.ShouldBe(0);
        phoneColumnCount.ShouldBe(0);
        firstNameColumnCount.ShouldBe(1);
        lastNameColumnCount.ShouldBe(1);
        phoneCountryCodeColumnCount.ShouldBe(1);
        phoneLocalNumberColumnCount.ShouldBe(1);
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
}
