using FluentMigrator;

namespace Aida.ParallelChange.Api.Infrastructure.Persistence.Migrations;

[Migration(202604010003)]
public sealed class RemoveLegacyFlatCustomerContactColumns : Migration
{
    public override void Up()
    {
        Delete.Column("phone").FromTable("customer_contacts");
        Delete.Column("contact_name").FromTable("customer_contacts");
    }

    public override void Down()
    {
        Alter.Table("customer_contacts")
            .AddColumn("contact_name").AsString(200).NotNullable().WithDefaultValue(string.Empty)
            .AddColumn("phone").AsString(30).NotNullable().WithDefaultValue(string.Empty);
    }
}
