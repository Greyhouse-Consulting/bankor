using Bancor.Core;
using Microsoft.EntityFrameworkCore;

namespace Bancor.Infrastructure
{
    public class BankOrContext : DbContext
    {
        public BankOrContext(DbContextOptions<BankOrContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        
        public DbSet<Transaction> Transactions { get; set; }

        public DbSet<Customer> Customers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().HasKey(k => k.Id);

            modelBuilder.Entity<Account>().HasKey(k => k.Id);
            modelBuilder.Entity<Account>().HasMany(p => p.Customers);
            
            modelBuilder.Entity<Customer>().HasKey(k => k.Id);
            modelBuilder.Entity<Customer>().HasMany(p => p.Accounts);

            base.OnModelCreating(modelBuilder);
        }
    }


    public class ContextFactory
    {
        public static BankOrContext Create()
        {
            var options = new DbContextOptionsBuilder<BankOrContext>()
                .UseInMemoryDatabase(databaseName: "BankOrContext")
                .Options;

            return new BankOrContext(options);
        }
    }
}