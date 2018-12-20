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
    public static class BankorDbFactory
    {
        public static string DatabaseName => "bancor";

        internal static string SqlLiteConnectionString = "Data Source={0};Mode=Memory;Cache=Shared";
        public static DatabaseFactory DbFactory { get; private set; }

        public static DatabaseFactory Setup(string name)
        {
            var fluentConfig = FluentMappingConfiguration.Configure(new NPocoLabMappings());

            DbFactory = DatabaseFactory.Config(x =>
            {
                var inMemoryDatabase = new Database(CreateConnection(name), DatabaseType.SQLite);

                x.UsingDatabase(() => inMemoryDatabase);
                x.WithFluentConfig(fluentConfig);
            });

            return DbFactory;
        }

        public static IDatabase Create(string name)
        {
            var factory = Setup(name);

            var db = factory.GetDatabase();

            db.Connection.Open();

            return db;
        }

        public static DbConnection CreateConnection(string name)
        {
            var connectionString = FormatConnectionString(name);
            return new SqliteConnection(connectionString);
        }

        public static string FormatConnectionString(string name)
        {
            return string.Format(SqlLiteConnectionString, name);
        }

        public static void Upgrade(string name)
        {
            var serviceProvider = CreateServices(name);

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

        public static IServiceProvider CreateServices(string name)
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSQLite()
                    .WithGlobalConnectionString(FormatConnectionString(name))
                    .ScanIn(typeof(SqlServerDatabaseFactory).Assembly)
                    .For.Migrations())
                .BuildServiceProvider();
        }
    }
}