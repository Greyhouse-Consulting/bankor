using System;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using BankOr.Core;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace AccountTransfer.Grains
{

    public interface INewAccountReceiver 
    {
        Task NewAccount(int id);
    }

    [StatelessWorker]
    [ImplicitStreamSubscription("RANDOMDATA")]
    public class AccoutCreatedObserverGrain : Grain, IAccoutCreatedObserverGrain, INewAccountReceiver
    {
        protected internal IAsyncStream<int> Stream;

        public Task NewAccount(int id)
        {
            return Task.CompletedTask;
        }

        public async Task StartSubscribe()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            Stream = streamProvider.GetStream<int>(StreamIdGenerator.StreamId, "ACCOUNTID");
            await Stream.SubscribeAsync(async (a, token) => await NewAccount(a));
        }
    }
}