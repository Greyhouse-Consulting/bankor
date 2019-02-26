﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Mongo2Go;
using MongoDB.Driver;

namespace Bancor.Integration.Tests
{
    public class MongoIntegrationTest
    {
        internal static MongoDbRunner _runner;
        internal static IMongoCollection<TestDocument> _collection;
        internal static string _databaseName = "IntegrationTest";
        internal static string _collectionName = "TestCollection";

        internal static void CreateConnection()
        {
            _runner = MongoDbRunner.Start(singleNodeReplSet: false);

            var client = new MongoClient(_runner.ConnectionString);
            var database = client.GetDatabase(_databaseName);
            _collection = database.GetCollection<TestDocument>(_collectionName);
        }
    }


    public static class TaskExtensions
    {
        public static async Task WithTimeout(this Task task, TimeSpan timeout)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cancellationTokenSource.Token));
                if (completedTask == task)
                {
                    cancellationTokenSource.Cancel();
                    await task;
                }
                else
                {
                    throw new TimeoutException("The operation has timed out.");
                }
            }
        }
    }
    internal class TestDocument
    {

        public Guid Id { get; set; }
        public static TestDocument DummyData1()
        {
            return new TestDocument
            {
                Name = "Kalle"
            };
        }

        public string Name { get; set; }
    }
}