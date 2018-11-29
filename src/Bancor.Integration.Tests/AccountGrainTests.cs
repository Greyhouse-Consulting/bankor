using System;
using System.Collections.Generic;
using System.Text;
using Bancor.Core.Grains;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.Extensions.Configuration;
using Orleans.TestingHost;
using Xunit;

namespace Bancor.Integration.Tests
{
    public class AccountGrainTests
    {
        [Fact]
        public void Should_Store_One_Account()
        {
            var testClusterOptions = new TestClusterOptions
            {
                
            };
            var readOnlyList = new ArraySegment<IConfigurationSource>
            {
            };
            var cluster = new TestCluster(testClusterOptions, readOnlyList);

            cluster.Deploy();

            var grain = cluster.GrainFactory.GetGrain<IAccountGrain>(2000);

            grain.HasNewName("Savings account");



        }
    }
}
