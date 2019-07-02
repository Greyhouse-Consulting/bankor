using Bancor.Core;
using MongoDB.Bson;

namespace Bancor.Infrastructure.MongoEntites
{
    public class MongoAccount
    {
        public ObjectId Id { get; set; }

        public Account Account { get; set; }
    }
}