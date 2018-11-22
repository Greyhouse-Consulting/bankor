using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace Bankor.Core.Grains
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