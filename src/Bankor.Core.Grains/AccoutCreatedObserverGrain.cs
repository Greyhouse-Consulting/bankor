using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace Bancor.Core.Grains
{

    public interface INewAccountReceiver 
    {
        Task NewAccount(string id);
    }

    [StatelessWorker]
    [ImplicitStreamSubscription("RANDOMDATA")]
    public class AccoutCreatedObserverGrain : Grain, IAccoutCreatedObserverGrain, INewAccountReceiver
    {
        protected internal IAsyncStream<string> Stream;

        public Task NewAccount(string id)
        {
            return Task.CompletedTask;
        }

        public async Task StartSubscribe()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            Stream = streamProvider.GetStream<string>(StreamIdGenerator.StreamId, "ACCOUNTID");
            await Stream.SubscribeAsync(async (a, token) => await NewAccount(a));
        }
    }
}