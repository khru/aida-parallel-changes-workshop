using Microsoft.Data.SqlClient;

namespace Aida.ParallelChange.Api.Tests.Integration;

internal static class SqlServerContainerReadinessProbe
{
    public static async Task WaitUntilAvailableAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var attempt = 0;

        while (attempt < 30)
        {
            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                return;
            }
            catch (SqlException)
            {
            }

            attempt++;
            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        await using var failedConnection = new SqlConnection(connectionString);
        await failedConnection.OpenAsync(cancellationToken);
    }
}
