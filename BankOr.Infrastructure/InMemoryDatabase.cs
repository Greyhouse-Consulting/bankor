using System;
using System.Data;
using System.Data.Common;
using BankOr.Core;
using Microsoft.Data.Sqlite;
using NPoco;
using NPoco.FluentMappings;
using Transaction = BankOr.Core.Transaction;

namespace BankOr.Infrastructure
{
    public class InMemoryDatabase : Database
    {

        public string ProviderName { get; set; }

        public DatabaseType DbType { get; set; }

        public void EnsureSharedConnectionConfigured()
        {
            Connection?.Open();
        }

        public void RecreateDataBase()
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine("Using SQLite In-Memory DB   ");
            Console.WriteLine("----------------------------");

            Connection.Open();

            var cmd = Connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE Accounts(Id INTEGER PRIMARY KEY, Name nvarchar(200), Balance REAL);";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE Customers(Id INTEGER PRIMARY KEY, Name nvarchar(200));";
            cmd.ExecuteNonQuery();
            cmd.CommandText = "CREATE TABLE Transactions(Id INTEGER PRIMARY KEY, Amount REAL, BookingDate DATE, AccountId INT);";
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

        public InMemoryDatabase(DbConnection connection) : base(connection)
        {
            DbType = DatabaseType.SQLite;
            ProviderName = DatabaseType.SQLite.GetProviderName();            
        }

        public InMemoryDatabase(DbConnection connection, DatabaseType dbType) : base(connection, dbType)
        {
        }

        public InMemoryDatabase(DbConnection connection, DatabaseType dbType, IsolationLevel? isolationLevel) 
            : base(connection, dbType, isolationLevel)
        {
        }

        public InMemoryDatabase(DbConnection connection, DatabaseType dbType, IsolationLevel? isolationLevel, bool enableAutoSelect) 
            : base(connection, dbType, isolationLevel, enableAutoSelect)
        {
        }

        public InMemoryDatabase(string connectionString, DatabaseType databaseType, DbProviderFactory provider) 
            : base(connectionString, databaseType, provider)
        {
        }

        public InMemoryDatabase(string connectionString, DatabaseType databaseType, DbProviderFactory provider, IsolationLevel? isolationLevel = null, bool enableAutoSelect = true) 
            : base(connectionString, databaseType, provider, isolationLevel, enableAutoSelect)
        {
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

    public class TransactionMapping : Map<Transaction>
    {
        public TransactionMapping()
        {
            PrimaryKey(k => k.Id);
            TableName("Transactions");

            Columns(x =>
            {
                x.Column(y => y.Amount).WithName("Amount");
                x.Column(y => y.BookingDate).WithName("BookingDate");
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
                var inMemoryDatabase = new InMemoryDatabase(CreateConnection());

                x.UsingDatabase(() => inMemoryDatabase);
                x.WithFluentConfig(fluentConfig);

                inMemoryDatabase.EnsureSharedConnectionConfigured();
                inMemoryDatabase.RecreateDataBase();
            });
        }

        public static DbConnection CreateConnection()
        {
           return new SqliteConnection("Data Source=:memory:");
        }
    }
}