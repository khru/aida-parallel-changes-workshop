using Aida.ParallelChange.Api.Infrastructure.Persistence.Migrations;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;
using Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Transition;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(runner => runner
                .AddSqlServer()
                .WithGlobalConnectionString(context.Configuration.GetConnectionString("SqlServer"))
                .ScanIn(typeof(CreateCustomerContactsTable).Assembly).For.Migrations());
    })
    .Build();

using var scope = host.Services.CreateScope();
var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
runner.MigrateUp();

var connectionString = configuration.GetConnectionString("SqlServer");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'SqlServer' is required.");
}

var configuredBatchSize = configuration.GetValue<int?>("Migration:BackfillBatchSize");
var backfillBatchSize = configuredBatchSize.GetValueOrDefault(100);

var connectionFactory = new DatabaseConnectionFactory(connectionString);
var backfillProcessor = new StructuredCustomerContactBackfillProcessor(connectionFactory);
var backfillResult = await backfillProcessor.BackfillAllAsync(backfillBatchSize);

Console.WriteLine($"backfill.total_rows={backfillResult.TotalRows}");
Console.WriteLine($"backfill.completed_rows={backfillResult.CompletedRows}");
Console.WriteLine($"backfill.skipped_rows={backfillResult.SkippedRows}");
