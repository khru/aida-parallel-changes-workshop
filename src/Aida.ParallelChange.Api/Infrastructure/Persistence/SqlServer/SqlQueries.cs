namespace Aida.ParallelChange.Api.Infrastructure.Persistence.SqlServer;

public static class SqlQueries
{
    public const string FindById = """
        SELECT customer_id AS CustomerId,
               email AS Email,
               first_name AS FirstName,
               last_name AS LastName,
               phone_country_code AS PhoneCountryCode,
               phone_local_number AS PhoneLocalNumber
        FROM customer_contacts
        WHERE customer_id = @CustomerId;
        """;

    public const string Create = """
        INSERT INTO customer_contacts
            (customer_id, email, first_name, last_name, phone_country_code, phone_local_number)
        VALUES
            (@CustomerId, @Email, @FirstName, @LastName, @PhoneCountryCode, @PhoneLocalNumber);
        """;

    public const string Update = """
        UPDATE customer_contacts
        SET email = @Email,
            first_name = @FirstName,
            last_name = @LastName,
            phone_country_code = @PhoneCountryCode,
            phone_local_number = @PhoneLocalNumber
        WHERE customer_id = @CustomerId;
        """;
}
