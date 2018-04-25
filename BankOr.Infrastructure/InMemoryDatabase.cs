using System;
using Microsoft.Data.Sqlite;
using NPoco;

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
            cmd.CommandText = "CREATE TABLE Accounts(Id INTEGER PRIMARY KEY, Name nvarchar(200));";
            cmd.ExecuteNonQuery();

            //cmd.CommandText = "CREATE TABLE ExtraUserInfos(ExtraUserInfoId INTEGER PRIMARY KEY, UserId int, Email nvarchar(200), Children int);";
            //cmd.ExecuteNonQuery();

            cmd.Dispose();
        }

        public void CleanupDataBase()
        {
            if (Connection == null) return;

            var cmd = Connection.CreateCommand();
            cmd.CommandText = "DROP TABLE Users;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE ExtraUserInfos;";
            cmd.ExecuteNonQuery();

            cmd.Dispose();
        }
    }
}