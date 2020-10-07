using Extractt.Web.Infra;
using Extractt.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.AddControllers();
            services.AddSingleton<ProcessDocument, ProcessDocument>();
            services.AddSingleton<PdfToText, PdfToText>();
            services.AddSingleton<ExtractionManager, ExtractionManager>();
            services.AddSingleton<FileManager, FileManager>();
            services.AddSingleton<Cognitive, Cognitive>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if(env.IsProduction())
                app.UseHttpsRedirection();

            app.UseCors("Cors");

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
