using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;

namespace Bancor.Api
{
    public class Startup
    {
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
                .UseLocalhostClustering()
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
