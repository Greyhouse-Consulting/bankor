using System;
using BankOr.Core;
using Microsoft.Data.Sqlite;
using NPoco;
using NPoco.FluentMappings;

namespace BankOr.Infrastructure
{
    public class InMemoryDatabase
    {
        public InMemoryDatabase()
        {
            DbType = DatabaseType.SQLite;
            ConnectionString = "Data Source=:memory:;Version=3;";
            ProviderName = DatabaseType.SQLite.GetProviderName();
            
            RecreateDataBase();
            EnsureSharedConnectionConfigured();
        }

        public string ProviderName { get; set; }

        public string ConnectionString { get; set; }

        public DatabaseType DbType { get; set; }

        public void EnsureSharedConnectionConfigured()
        {
            if (Connection != null) return;
            

            Connection = new Microsoft.Data.Sqlite.SqliteConnection(ConnectionString);
            Connection.Open();
        }

        public SqliteConnection Connection { get; set; }

        public void RecreateDataBase()
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine("Using SQLite In-Memory DB   ");
            Console.WriteLine("----------------------------");


            var cmd = Connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE Accounts(Id INTEGER PRIMARY KEY, Name nvarchar(200), Balance REAL);";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE Customers(Id INTEGER PRIMARY KEY, Name nvarchar(200));";
            cmd.ExecuteNonQuery();

            //cmd.CommandText = "CREATE TABLE ExtraUserInfos(ExtraUserInfoId INTEGER PRIMARY KEY, UserId int, Email nvarchar(200), Children int);";
            //cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        public void CleanupDataBase()
        {
            if (Connection == null) return;

            var cmd = Connection.CreateCommand();
            cmd.CommandText = "DROP TABLE Customers;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE Accounts;";
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }
    }


    public class AccountMapping : Map<Account>
    {
        public AccountMapping()
        {
            PrimaryKey(k => k.Id);
            TableName("Accounts");

            Columns(x =>
            {
                x.Column(y => y.Balance).WithName("Balance");
                x.Column(y => y.Name).WithName("Name");
            });
        }
    }
    
    public class CustomerMapping : Map<Customer>
    {
        public CustomerMapping()
        {
            PrimaryKey(k => k.Id);
            TableName("Customers");

            Columns(x =>
            {
                x.Column(y => y.Name).WithName("Name");
            });
        }
    }

    public static class BankorDbFactory
    {
        public static DatabaseFactory DbFactory { get; private set; }

        public static void Setup()
        {
            var fluentConfig = FluentMappingConfiguration.Configure(new AccountMapping(), new CustomerMapping());

            DbFactory = DatabaseFactory.Config(x =>
            {
                x.UsingDatabase(() => new Database(new InMemoryDatabase().Connection));
                x.WithFluentConfig(fluentConfig);
            });
        }
    }
}