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
            Console.WriteLine($"Version: 0.17");
            EnvironmentVariables.AccessKey = Environment.GetEnvironmentVariable("ACCESS_KEY");
            Console.WriteLine($"AccessKey: {EnvironmentVariables.AccessKey}");
            EnvironmentVariables.CognitiveKey = Environment.GetEnvironmentVariable("COGNITIVE_KEY");
            Console.WriteLine($"CognitiveKey: {EnvironmentVariables.CognitiveKey}");
            EnvironmentVariables.CognitiveApi = Environment.GetEnvironmentVariable("COGNITIVE_API");
            Console.WriteLine($"CognitiveApi: {EnvironmentVariables.CognitiveApi}");
            EnvironmentVariables.HangfireConnection = Environment.GetEnvironmentVariable("HANGFIRE_CONNECTION");
            Console.WriteLine($"HangfireConnection: {EnvironmentVariables.HangfireConnection}");
            EnvironmentVariables.HangfireUser = Environment.GetEnvironmentVariable("HANGFIRE_USER") ?? "admin";
            Console.WriteLine($"HangfireUser: {EnvironmentVariables.HangfireUser}");
            EnvironmentVariables.HangfirePassword = Environment.GetEnvironmentVariable("HANGFIRE_PASSWORD") ?? "hangfire";
            Console.WriteLine($"HangfirePassword: {EnvironmentVariables.HangfirePassword}");
            if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NumberMaxDocumentsInParallel")))
                EnvironmentVariables.NumberMaxDocumentsInParallel = int.Parse(Environment.GetEnvironmentVariable("NumberMaxDocumentsInParallel"));
            else
                EnvironmentVariables.NumberMaxDocumentsInParallel = 5;
            Console.WriteLine($"NumberMaxDocumentsInParallel: {EnvironmentVariables.NumberMaxDocumentsInParallel}");

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}
