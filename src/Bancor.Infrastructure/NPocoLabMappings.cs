using Bancor.Core;
using NPoco.FluentMappings;

namespace Bancor.Infrastructure
{
    public class NPocoLabMappings : Mappings
    {
        public NPocoLabMappings()
        {
            MapCustomers();
            MapAccounts();
            CustomerAccountMapping();
            TransactionMapping();
        }

        private void MapAccounts()
        {
            For<Account>().PrimaryKey(k => k.Id, false);
            For<Account>().TableName("Accounts");

            For<Account>().Columns(x =>
            {
                x.Column(y => y.Balance).WithName("Balance");
                x.Column(y => y.Name).WithName("Name");
                x.Column(y => y.Transactions).Ignore();
                x.Column(y => y.Customers).Ignore();
            });
        }

        private void MapCustomers()
        {
            For<Customer>().PrimaryKey(k => k.Id, false);

            For<Customer>().TableName("Customers");

            For<Customer>().Columns(x =>
            {
                x.Column(y => y.Id);
                x.Column(y => y.Name);
                x.Column(y => y.Accounts).Ignore();
            });
        }

        public void CustomerAccountMapping()
        {

            For<CustomerAccount>().CompositePrimaryKey(k => k.CustomerId, k => k.AccountId);

            For<CustomerAccount>().TableName("Customers_Accounts");

            For<CustomerAccount>().Columns(x =>
            {
                x.Column(y => y.CustomerId);
                x.Column(y => y.AccountId);
            });
        }

        public void TransactionMapping()
        {
            For<Transaction>().PrimaryKey(k => k.Id);
            For<Transaction>().TableName("Transactions");

            For<Transaction>().Columns(x =>
            {
                x.Column(y => y.Amount).WithName("Amount");
                x.Column(y => y.BookingDate).WithName("BookingDate");
                x.Column(y => y.AccountId).WithName("AccountId");
            });
        }
    }
}