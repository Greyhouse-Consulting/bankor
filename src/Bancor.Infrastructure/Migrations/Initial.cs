using FluentMigrator;

namespace Bancor.Infrastructure.Migrations
{
    [Migration(201811261732000)]
    public class Initial : Migration
    {
        public override void Up()
        {
            Create.Table("Customers")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("Name").AsString();

            Create.Table("AccountsIds")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("Name").AsString()
                .WithColumn("Balance").AsDecimal();

            Create.Table("Customers_Accounts")
                .WithColumn("CustomerId").AsInt64().PrimaryKey()
                .WithColumn("AccountId").AsInt64().PrimaryKey();

            Create.Table("Transactions")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("AccountId").AsInt64()
                .WithColumn("Created").AsBoolean();

        }

        public override void Down()
        {
            Delete.Table("CustomerAccounts");
            Delete.Table("Customers");
            Delete.Table("AccountsIds");
            Delete.Table("Transactions");
        }
    }
}