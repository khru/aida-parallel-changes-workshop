namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;

public static class SqlQueries
{
    public const string FindById = """
        SELECT customer_id AS CustomerId,
               contact_name AS ContactName,
               phone AS Phone,
               email AS Email
        FROM customer_contacts
        WHERE customer_id = @CustomerId;
        """;

    public const string Create = """
        INSERT INTO customer_contacts (customer_id, contact_name, phone, email)
        VALUES (@CustomerId, @ContactName, @Phone, @Email);
        """;

    public const string Update = """
        UPDATE customer_contacts
        SET contact_name = @ContactName,
            phone = @Phone,
            email = @Email
        WHERE customer_id = @CustomerId;
        """;
}
