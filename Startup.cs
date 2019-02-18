using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ObjectLogging.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer;

namespace ObjectLogging
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // configure Serilog
            var connectionString = Configuration.GetConnectionString("dbConnection");
            var logTable = "Logs";

            var columnOptions = new ColumnOptions()
            {
                AdditionalDataColumns = new List<DataColumn> {
                    new DataColumn {DataType = typeof(string), ColumnName = "ServerName"},
                    new DataColumn {DataType = typeof(string), ColumnName = "ClientName"}
                }
            };

            var loggerConfig = new LoggerConfiguration()
                .WriteTo.MSSqlServer(connectionString, logTable, columnOptions: columnOptions)
                .Enrich.With<ColumnsEnricher>();

            var logger = loggerConfig.CreateLogger();
            loggerFactory.AddSerilog(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
