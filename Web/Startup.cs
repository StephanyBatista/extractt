using System;
using Extractt.Web.Infra;
using Extractt.Web.Services;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Infra;

namespace Extractt
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
            services.AddCors(options =>
            {
                options.AddPolicy("Cors",
                    builder => builder.AllowAnyOrigin());
            });
            ConfigureHangfire(services);

            services.AddHangfireServer(options => options.WorkerCount = 2);

            services.AddControllers();
            services.AddSingleton<ProcessDocument, ProcessDocument>();
            services.AddSingleton<PdfToText, PdfToText>();
            services.AddSingleton<ExtractionManager, ExtractionManager>();
            services.AddSingleton<FileManager, FileManager>();
            services.AddSingleton<Cognitive, Cognitive>();
            services.AddSingleton<Callback, Callback>();
        }

        private static void ConfigureHangfire(IServiceCollection services)
        {
            var useSqlServer = !string.IsNullOrEmpty(EnvironmentVariables.HangfireConnection);
            if (useSqlServer)
            {
                services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(EnvironmentVariables.HangfireConnection, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(60),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                }));
            }
            else
            {
                services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMemoryStorage());
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors("Cors");

            app.UseRouting();

            var dashboardOption = new DashboardOptions
            {
                Authorization = new[] {
                    new HangfireCustomBasicAuthenticationFilter{ 
                        User= EnvironmentVariables.HangfireUser, Pass= EnvironmentVariables.HangfirePassword 
                    }
                }
            };
            app.UseHangfireDashboard("/hangfire", dashboardOption);

            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
