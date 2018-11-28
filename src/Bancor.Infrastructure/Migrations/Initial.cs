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
                .WithColumn("Name").AsString()
                .WithColumn("Created").AsBoolean();

            Create.Table("Accounts")
                .WithColumn("Id").AsInt64().PrimaryKey()
                .WithColumn("Name").AsString()
                .WithColumn("Balance").AsDecimal()
                .WithColumn("Created").AsBoolean();

            Create.Table("Customers_Accounts")
                .WithColumn("CustomerId").AsInt64().PrimaryKey()
                .WithColumn("AccountId").AsInt64().PrimaryKey();

            Create.Table("Transactions")
                .WithColumn("Id").AsGuid().PrimaryKey()
                .WithColumn("AccountId").AsInt64()
                .WithColumn("Created").AsBoolean()
                .WithColumn("Amount").AsDecimal();

            //Create.ForeignKey("customer_accounts_FK")
            //    .FromTable("Accounts").InSchema("dob").ForeignColumn("Id")
            //    .ToTable("Customer_Accounts").InSchema("dbo").PrimaryColumn("AccountId");

            //Create.ForeignKey("customer_accounts_FK")
            //    .FromTable("Customers").InSchema("dob").ForeignColumn("Id")
            //    .ToTable("Customer_Accounts").InSchema("dbo").PrimaryColumn("CustomerId");
        }

        public override void Down()
        {
            Delete.Table("CustomerAccounts");
            Delete.Table("Customers");
            Delete.Table("Accounts");
            Delete.Table("Transactions");
        }
    }
}