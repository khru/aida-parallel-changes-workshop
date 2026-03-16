using Dapper;

namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer.Transition;

public sealed class StructuredCustomerContactBackfillProcessor
{
    private const int DefaultBatchSize = 100;

    private readonly DatabaseConnectionFactory _connectionFactory;

    public StructuredCustomerContactBackfillProcessor(DatabaseConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<BackfillBatchResult> BackfillBatchAsync(int batchSize = DefaultBatchSize, CancellationToken cancellationToken = default)
    {
        var effectiveBatchSize = batchSize > 0 ? batchSize : DefaultBatchSize;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var command = new CommandDefinition(
            BackfillBatchSql,
            new { BatchSize = effectiveBatchSize },
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<BackfillBatchResult>(command);
    }

    public async Task<BackfillRunResult> BackfillAllAsync(int batchSize = DefaultBatchSize, CancellationToken cancellationToken = default)
    {
        var effectiveBatchSize = batchSize > 0 ? batchSize : DefaultBatchSize;
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        var countCommand = new CommandDefinition(CountRowsSql, cancellationToken: cancellationToken);
        var initialCounters = await connection.QuerySingleAsync<BackfillInitialCounters>(countCommand);

        if (initialCounters.PendingRows == 0)
        {
            return new BackfillRunResult(0, initialCounters.TotalRows, initialCounters.TotalRows);
        }

        var totalUpdatedRows = 0;
        var remainingRows = initialCounters.PendingRows;

        while (remainingRows > 0)
        {
            var batchResult = await BackfillBatchAsync(effectiveBatchSize, cancellationToken);
            totalUpdatedRows += batchResult.UpdatedRows;
            remainingRows = batchResult.RemainingRows;

            if (batchResult.UpdatedRows == 0)
            {
                break;
            }
        }

        var skippedRows = initialCounters.TotalRows - totalUpdatedRows;
        return new BackfillRunResult(totalUpdatedRows, skippedRows, initialCounters.TotalRows);
    }

    private const string BackfillBatchSql = """
        DECLARE @rows_to_backfill TABLE (customer_id INT NOT NULL PRIMARY KEY);

        INSERT INTO @rows_to_backfill (customer_id)
        SELECT TOP (@BatchSize) customer_id
        FROM customer_contacts
        WHERE first_name IS NULL
           OR last_name IS NULL
           OR phone_country_code IS NULL
           OR phone_local_number IS NULL
        ORDER BY customer_id;

        DECLARE @selected_rows INT = @@ROWCOUNT;

        UPDATE contacts
        SET first_name = COALESCE(
                contacts.first_name,
                CASE
                    WHEN CHARINDEX(' ', LTRIM(RTRIM(contacts.contact_name))) > 0
                        THEN LEFT(LTRIM(RTRIM(contacts.contact_name)), CHARINDEX(' ', LTRIM(RTRIM(contacts.contact_name))) - 1)
                    ELSE LTRIM(RTRIM(contacts.contact_name))
                END),
            last_name = COALESCE(
                contacts.last_name,
                CASE
                    WHEN CHARINDEX(' ', LTRIM(RTRIM(contacts.contact_name))) > 0
                        THEN LTRIM(SUBSTRING(LTRIM(RTRIM(contacts.contact_name)), CHARINDEX(' ', LTRIM(RTRIM(contacts.contact_name))) + 1, LEN(LTRIM(RTRIM(contacts.contact_name)))))
                    ELSE ''
                END),
            phone_country_code = COALESCE(
                contacts.phone_country_code,
                CASE
                    WHEN CHARINDEX(' ', LTRIM(RTRIM(contacts.phone))) > 0
                        THEN LEFT(LTRIM(RTRIM(contacts.phone)), CHARINDEX(' ', LTRIM(RTRIM(contacts.phone))) - 1)
                    ELSE LTRIM(RTRIM(contacts.phone))
                END),
            phone_local_number = COALESCE(
                contacts.phone_local_number,
                CASE
                    WHEN CHARINDEX(' ', LTRIM(RTRIM(contacts.phone))) > 0
                        THEN LTRIM(SUBSTRING(LTRIM(RTRIM(contacts.phone)), CHARINDEX(' ', LTRIM(RTRIM(contacts.phone))) + 1, LEN(LTRIM(RTRIM(contacts.phone)))))
                    ELSE ''
                END)
        FROM customer_contacts contacts
        INNER JOIN @rows_to_backfill rows
            ON contacts.customer_id = rows.customer_id;

        DECLARE @updated_rows INT = @@ROWCOUNT;

        SELECT
            @updated_rows AS UpdatedRows,
            CASE
                WHEN @selected_rows >= @updated_rows THEN @selected_rows - @updated_rows
                ELSE 0
            END AS SkippedRows,
            (
                SELECT COUNT(*)
                FROM customer_contacts
                WHERE first_name IS NULL
                   OR last_name IS NULL
                   OR phone_country_code IS NULL
                   OR phone_local_number IS NULL
            ) AS RemainingRows;
        """;

    private const string CountRowsSql = """
        SELECT
            COUNT(*) AS TotalRows,
            SUM(CASE
                    WHEN first_name IS NULL
                      OR last_name IS NULL
                      OR phone_country_code IS NULL
                      OR phone_local_number IS NULL
                    THEN 1
                    ELSE 0
                END) AS PendingRows
        FROM customer_contacts;
        """;
}

public sealed record BackfillBatchResult(int UpdatedRows, int SkippedRows, int RemainingRows);

public sealed record BackfillRunResult(int CompletedRows, int SkippedRows, int TotalRows);

internal sealed record BackfillInitialCounters(int TotalRows, int PendingRows);
