using System.IO;
using System.Reflection;
using Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SphinxConnector.FluentApi;
using SphinxConnectorSample.Config;
using SphinxConnectorSample.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace SphinxConnectorSample
{
    public class Startup
    {
        private static readonly string _apiTitle = "Sphinx Connector Test API";
        private static readonly string _apiVersion = "v1";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddOptions();
            services.AddSingleton<IFulltextStore>(new FulltextStore().Initialize());
            services.AddSingleton<IQueryBuilder, QueryBuilder>();
            services.AddSingleton<ISphinxService, SphinxService>();
            services.Configure<SphinxConfig>(opt => builder.GetSection("Sphinx").Bind(opt));

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(_apiVersion, new Info { Title = _apiTitle, Version = _apiVersion });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(filePath);
                });


            //Logging for SphinxConnector
            LogManager.Adapter = new Common.Logging.Simple.DebugLoggerFactoryAdapter
            {
                Level = Common.Logging.LogLevel.All,
                ShowLevel = true,
                ShowDateTime = true,
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"sphinx connector test api"); });
        }
    }
}
