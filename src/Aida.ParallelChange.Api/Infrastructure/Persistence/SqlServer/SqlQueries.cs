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

    public const string Upsert = """
        MERGE customer_contacts AS target
        USING (
            SELECT @CustomerId AS customer_id,
                   @ContactName AS contact_name,
                   @Phone AS phone,
                   @Email AS email
        ) AS source
        ON target.customer_id = source.customer_id
        WHEN MATCHED THEN
            UPDATE SET contact_name = source.contact_name,
                       phone = source.phone,
                       email = source.email
        WHEN NOT MATCHED THEN
            INSERT (customer_id, contact_name, phone, email)
            VALUES (source.customer_id, source.contact_name, source.phone, source.email);
        """;
}
