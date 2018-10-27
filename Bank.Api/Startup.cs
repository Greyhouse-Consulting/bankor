using System;
using BankOr.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans;

namespace Bank.Api
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
            BankorDbFactory.Setup();
            var conn = BankorDbFactory.CreateConnection();
            var db = new InMemoryDatabase(conn);

            db.EnsureSharedConnectionConfigured();
            //db.RecreateDataBase();

            services.AddSingleton(db);
            services.AddMvc();
            var clusterClient = ClientFactory.StartClientWithRetries().Result;
            services.AddSingleton<IClusterClient>(clusterClient);

            return services.BuildServiceProvider();
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
