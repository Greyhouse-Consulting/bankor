using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using AccountTransfer.Grains;
using AccountTransfer.Interfaces;
using Microsoft.Data.Sqlite;
using Moq;
using NPoco;
using Orleans;
using Orleans.Core;
using Orleans.Runtime;
using Pose;
using Xunit;

namespace BankOr.Infrastructure.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

            BankorDbFactory.Setup();
            var conn = BankorDbFactory.CreateConnection();
            var db = new InMemoryDatabase(conn);

            db.EnsureSharedConnectionConfigured();
            db.RecreateDataBase();
            var provider = new BankOrStorageProvider(db);


            var gi = new Mock<IGrainIdentity>();

            //gi.Setup(x => x.I)

            var graindRef = new Mock<IAccountGrain>();

            //var s = Shim.Replace(() => gi.Identity).With(() => (long)2000);

            var ags = new AccountGrainState
            {
                Balance = 3000,
                Transactions = new List<Core.Transaction>
                {
                    new Core.Transaction
                    {
                        AccountId = 2000,
                        Amount = 3000,
                        BookingDate = new DateTime(2018, 1, 1, 1, 1, 1),
                        Id = 7000
                    }
                }
            };


            //PoseContext.Isolate(() =>
            //{
                provider.WriteStateAsync("AccountTransfer.Grains.AccountGrain",
                    graindRef.Object as GrainReference, ags as IGrainState).Wait();
            //}, s);
        }
    }

    public interface IMockAccountGrain : IAccountGrain
    {


        long GetPrimaryKeyLong();

    }

    public class MockAccountGrain : IMockAccountGrain
    {
        public Task Withdraw(uint amount)
        {
            throw new NotImplementedException();
        }

        public Task Deposit(uint amount)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetBalance()
        {
            throw new NotImplementedException();
        }

        public Task Owner(string userId)
        {
            throw new NotImplementedException();
        }

        public long GetPrimaryKeyLong()
        {
            return 200;
        }
    }

}
