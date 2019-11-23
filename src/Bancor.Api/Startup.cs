using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using IHostingEnvironment = Microsoft.Extensions.Hosting.IHostingEnvironment;

namespace Bancor.Api
{
    public class Startup
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private int _numRetries;
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            var builder = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "bancor-api";
                })
                .ConfigureLogging(logging => logging.AddConsole());

            if (_hostingEnvironment.IsEnvironment("Integration"))
            {
                builder.UseConsulClustering(options =>
                {
                    options.Address = new Uri("http://consul:8500");
                });
            }
            else if(_hostingEnvironment.IsDevelopment())
            {
                builder.UseLocalhostClustering();
            }

            var client = builder.Build();

            client.Connect(RetryFilter).GetAwaiter().GetResult();

            //var clusterClient = ClientFactory.StartClientWithRetries().GetAwaiter().GetResult();

            services.AddSingleton<IClusterClient>(client);


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bancor API", Version = "v1" });
            });

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

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseRouting();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
