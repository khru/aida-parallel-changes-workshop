using FluentMigrator;

namespace Aida.ParallelChange.Api.Infrastructure.Persistence.Migrations;

[Migration(202604010001)]
public sealed class CreateCustomerContactsTable : Migration
{
    public override void Up()
    {
        Create.Table("customer_contacts")
            .WithColumn("customer_id").AsInt32().PrimaryKey()
            .WithColumn("contact_name").AsString(200).NotNullable()
            .WithColumn("phone").AsString(30).NotNullable()
            .WithColumn("email").AsString(200).NotNullable();
    }

    public override void Down()
    {
        Delete.Table("customer_contacts");
    }
}
