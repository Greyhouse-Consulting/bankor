using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Bancor.Api
{
    public class Startup
    {
        private int _numRetries;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider  ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
               var client = new ClientBuilder()
                .UseConsulClustering(options =>
                {
                    options.Address = new Uri("http://consul:8500", UriKind.Absolute);
                })
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "bancor";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();
            
            client.Connect(RetryFilter).GetAwaiter().GetResult();

            //var clusterClient = ClientFactory.StartClientWithRetries().GetAwaiter().GetResult();

            services.AddSingleton<IClusterClient>(client);

            return services.BuildServiceProvider();
        }

        private Task<bool> RetryFilter(Exception arg)
        {
            if (_numRetries > 10)
                return Task.FromResult(false);

            _numRetries += 1;
            Thread.Sleep(5000);
            return Task.FromResult(true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
