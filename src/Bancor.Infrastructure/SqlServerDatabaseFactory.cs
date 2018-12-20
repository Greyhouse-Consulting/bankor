using System;
using System.Data.Common;
using System.Data.SqlClient;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using NPoco;
using NPoco.FluentMappings;

namespace Bancor.Infrastructure
{
    public class SqlServerDatabaseFactory
    {
        private static DatabaseFactory _factory = null;
        protected internal static string ServerLocalhostDatabaseBancorTrustedConnectionTrue = "Server=db;Database=master;User=sa;Password=MyPassword001;";

        public static IDatabase Create()
        {
            EnsureDatabaseFactoryCreated();

            return _factory.GetDatabase();
        }

        private static void EnsureDatabaseFactoryCreated()
        {
            if (_factory != null)
                return;

            var fluentConfig = FluentMappingConfiguration.Configure(new NPocoLabMappings());

            _factory = DatabaseFactory.Config(x =>
            {
                var db = new Database(CreateConnection(), DatabaseType.SqlServer2012);

                x.UsingDatabase(() => db);
                x.WithFluentConfig(fluentConfig);
            });
        }

        public static DbConnection CreateConnection()
        {
            var connection = new SqlConnection(ServerLocalhostDatabaseBancorTrustedConnectionTrue);

            connection.Open();

            return connection;
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
                    .AddSqlServer2016()
                    .WithGlobalConnectionString(ServerLocalhostDatabaseBancorTrustedConnectionTrue)
                    .ScanIn(typeof(SqlServerDatabaseFactory).Assembly)
                    .For.Migrations()
                )
                .BuildServiceProvider();
        }
    }
}