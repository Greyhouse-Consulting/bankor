using System;
using Bancor.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Bancor.Infrastructure.MongoEntites
{
    public class MongoCustomer
    {
        [BsonId]
        public Guid Id { get; set; }

        public Customer Customer { get; set; }
    }
}