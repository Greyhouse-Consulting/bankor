using MongoDB.Driver;

namespace Bancor.Infrastructure.Abstractions
{
    public interface IMongoDbFactory
    {
        IMongoDatabase Create();
    }
}