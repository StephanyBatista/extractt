using System;
using Extractt.Web.Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Extractt
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            EnvironmentVariables.AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
            EnvironmentVariables.HangfireConnection = Environment.GetEnvironmentVariable("HANGFIRE_CONNECTION");
            EnvironmentVariables.HangfireUser = Environment.GetEnvironmentVariable("HANGFIRE_USER");
            EnvironmentVariables.HangfirePassword = Environment.GetEnvironmentVariable("HANGFIRE_PASSWORD");
            EnvironmentVariables.CognitiveKey = Environment.GetEnvironmentVariable("COGNITIVE_KEY");
            EnvironmentVariables.CognitiveApi = Environment.GetEnvironmentVariable("COGNITIVE_API");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
