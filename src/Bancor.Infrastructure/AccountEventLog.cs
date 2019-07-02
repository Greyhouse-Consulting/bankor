using System;
using Bancor.Core.Events.Account;
using MongoDB.Bson.Serialization.Attributes;

namespace Bancor.Infrastructure
{

    public class AccountEventLog
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public AccountEvent AccountEvent { get; set;}
        public int AccountVersion { get; set; }


        public AccountEventLog(Guid accountId, AccountEvent accountEvent, int version)
        {
            AccountId = accountId;
            AccountEvent = accountEvent;
            AccountVersion = version;
        }
    }
}