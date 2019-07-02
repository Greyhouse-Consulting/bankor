using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Bancor.Core.Events.Account
{

    public abstract class AccountEvent
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set;}

        public string Description { get; set; }

        protected AccountEvent(string description)
        {
            Created = DateTime.Now;
            Id = Guid.NewGuid();
            Description = description;
        }

    }
}