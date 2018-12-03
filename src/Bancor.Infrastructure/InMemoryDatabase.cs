using System;
using System.Data;
using System.Data.Common;
using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using NPoco;
using NPoco.FluentMappings;

namespace Bancor.Infrastructure
{
    //public class InMemoryDatabase : Database
    //{

    //    public string ProviderName { get; set; }

    //    public DatabaseType DbType { get; set; }

    //    public void EnsureSharedConnectionConfigured()
    //    {
    //        Connection?.Open();
    //    }

    //    public void RecreateDataBase()
    //    {
    //        Console.WriteLine("----------------------------");
    //        Console.WriteLine("Using SQLite In-Memory DB   ");
    //        Console.WriteLine("----------------------------");

    //        Connection.Open();

    //        var cmd = Connection.CreateCommand();
    //        cmd.CommandText = "CREATE TABLE Accounts(Id INTEGER, Name nvarchar(200), Balance REAL, Created INTEGER, PRIMARY KEY (Id));";
    //        cmd.ExecuteNonQuery();
    //        cmd.CommandText = "CREATE TABLE Customers(Id INTEGER , Name nvarchar(200), Created INTEGER, PRIMARY KEY (Id));";
    //        cmd.ExecuteNonQuery();
    //        cmd.CommandText = "CREATE TABLE Customers_Accounts(CustomerId INTEGER , AccountId INTEGER, PRIMARY KEY (CustomerId, AccountId));";
    //        cmd.ExecuteNonQuery();
    //        cmd.CommandText = "CREATE TABLE Transactions(Id STRING PRIMARY KEY, Amount REAL, BookingDate DATE, AccountId INT);";
    //        cmd.ExecuteNonQuery();

    //        //cmd.CommandText = "CREATE TABLE ExtraUserInfos(ExtraUserInfoId INTEGER PRIMARY KEY, UserId int, Email nvarchar(200), Children int);";
    //        //cmd.ExecuteNonQuery();

    //        cmd.Dispose();
    //    }

    //    public void CleanupDataBase()
    //    {
    //        if (Connection == null) return;

    //        var cmd = Connection.CreateCommand();
    //        cmd.CommandText = "DROP TABLE Customers;";
    //        cmd.ExecuteNonQuery();

    //        cmd.CommandText = "DROP TABLE Accounts;";
    //        cmd.ExecuteNonQuery();

    //        cmd.Dispose();
    //    }

    //    public InMemoryDatabase(DbConnection connection) : base(connection)
    //    {
    //        DbType = DatabaseType.SQLite;
    //        ProviderName = DatabaseType.SQLite.GetProviderName();            
    //    }

    //    public InMemoryDatabase(DbConnection connection, DatabaseType dbType) : base(connection, dbType)
    //    {
    //    }

    //    public InMemoryDatabase(DbConnection connection, DatabaseType dbType, IsolationLevel? isolationLevel) 
    //        : base(connection, dbType, isolationLevel)
    //    {
    //    }

    //    public InMemoryDatabase(DbConnection connection, DatabaseType dbType, IsolationLevel? isolationLevel, bool enableAutoSelect) 
    //        : base(connection, dbType, isolationLevel, enableAutoSelect)
    //    {
    //    }

    //    public InMemoryDatabase(string connectionString, DatabaseType databaseType, DbProviderFactory provider) 
    //        : base(connectionString, databaseType, provider)
    //    {
    //    }

    //    public InMemoryDatabase(string connectionString, DatabaseType databaseType, DbProviderFactory provider, IsolationLevel? isolationLevel = null, bool enableAutoSelect = true) 
    //        : base(connectionString, databaseType, provider, isolationLevel, enableAutoSelect)
    //    {
    //    }
    //}



    public static class BankorDbFactory
    {
        internal static string SqlLiteConnectionString = "Data Source=sharedmemdb;Mode=Memory;Cache=Shared";
        public static DatabaseFactory DbFactory { get; private set; }

        public static DatabaseFactory Setup()
        {
            var fluentConfig = FluentMappingConfiguration.Configure(new NPocoLabMappings());

            DbFactory = DatabaseFactory.Config(x =>
            {
                var inMemoryDatabase = new Database(CreateConnection(), DatabaseType.SQLite);

                x.UsingDatabase(() => inMemoryDatabase);
                x.WithFluentConfig(fluentConfig);
            });

            return DbFactory;
        }

        public static IDatabase Create()
        {
            var factory = Setup();

            var db = factory.GetDatabase();

            db.Connection.Open();

            return db;
        }

        public static DbConnection CreateConnection()
        {
            return new SqliteConnection(SqlLiteConnectionString);
        }

        public static void Upgrade()
        {
            var serviceProvider = CreateServices();

            using (var scope = serviceProvider.CreateScope())
            {
                UpdateDatabase(scope.ServiceProvider);
            }
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        public static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(SqlLiteConnectionString)
                    .ScanIn(typeof(SqlServerDatabaseFactory).Assembly)
                    .For.Migrations())
                .BuildServiceProvider();
        }
    }
}