using MongoDB.Driver;
using Shouldly;
using Xunit;

namespace Bancor.Integration.Tests
{
    public class MongoInMemoryTest : MongoIntegrationTest
    {
        [Fact]
        public  void Should_Load_One_Document()
        {
            CreateConnection();

            _collection.InsertOne(TestDocument.DummyData1());

            var result = _collection.FindSync(t => true).First();

            result.Name.ShouldBe("Kalle");

            _runner.Dispose();
        }

    }
}