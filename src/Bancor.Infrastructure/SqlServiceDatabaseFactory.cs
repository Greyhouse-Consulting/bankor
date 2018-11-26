using System;
using System.Data.Common;
using System.Data.SqlClient;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using NPoco;
using NPoco.FluentMappings;

namespace Bancor.Infrastructure
{
    public class SqlServiceDatabaseFactory
    {
        private static DatabaseFactory _factory = null;
        protected internal static string ServerLocalhostDatabaseBancorTrustedConnectionTrue = "Server=localhost;Database=bancor;Trusted_Connection=True;";

        public Database Create()
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
            return new SqlConnection(ServerLocalhostDatabaseBancorTrustedConnectionTrue);
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

            // Execute the migrations
            runner.MigrateUp();
        }

        public static IServiceProvider CreateServices()
        {
            return new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddSqlServer2016()
                    .WithGlobalConnectionString(ServerLocalhostDatabaseBancorTrustedConnectionTrue)
                    .ScanIn(typeof(SqlServiceDatabaseFactory).Assembly)
                    .For.Migrations()
                )
                .BuildServiceProvider();
        }
    }
}