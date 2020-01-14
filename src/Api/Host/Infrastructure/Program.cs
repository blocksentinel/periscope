using System;
using System.IO;
using System.Threading.Tasks;
using Cinder.Core;
using Cinder.Core.Exceptions;
using Cinder.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Cinder.Api.Host.Infrastructure
{
    public class Program
    {
        public static IConfiguration Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            try
            {
                Log.Information("Starting API; Version: {Version}; Build Date: {BuildDate}", VersionInfo.Version,
                    VersionInfo.BuildDate);

                IWebHost host = BuildWebHost(args);
                await host.RunAsync().AnyContext();

                return 0;
            }
            catch (Exception e)
            {
                if (!(e is LoggedException))
                {
                    Log.Fatal(e, "API terminated unexpectedly");
                }

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseConfiguration(Configuration).UseStartup<Startup>().UseSerilog().Build();
        }
    }
}
