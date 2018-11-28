using System;
using System.Collections.Generic;
using System.Text;
using FluentMigrator;

namespace Bancor.Infrastructure.Migrations
{
    [Migration(201811281732000)]
    public class RemoveCreatedColumn  : Migration
    {
        public override void Up()
        {
            Delete.Column("Created").FromTable("Accounts");
            Delete.Column("Created").FromTable("Customers");
            Delete.Column("Created").FromTable("Transactions");
        }

        public override void Down()
        {
            Create.Column("Created").OnTable("Accounts");
            Create.Column("Created").OnTable("Customers");
            Create.Column("Created").OnTable("Transactions");
        }
    }
}
