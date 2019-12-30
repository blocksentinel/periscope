using System;
using System.Threading.Tasks;
using Cinder.Core;
using Cinder.Core.Exceptions;
using Cinder.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cinder.Indexers.HostBase
{
    public class HostWrapper
    {
        public IHostBuilder DefaultBuilder = new HostBuilder();

        public async Task<int> Run(string name, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

            try
            {
                Log.Information("Starting {Name}; Version: {Version}; Build Date: {BuildDate}", name, VersionInfo.Version,
                    VersionInfo.BuildDate);
                IHost host = DefaultBuilder.UseSerilog().Build();

                await host.RunAsync().AnyContext();

                return 0;
            }
            catch (Exception e)
            {
                if (!(e is LoggedException))
                {
                    Log.Fatal(e, "Address Indexer terminated unexpectedly");
                }

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
