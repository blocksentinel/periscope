using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Periscope.Core;
using Periscope.Core.Exceptions;
using Periscope.Core.Extensions;
using Serilog;

namespace Periscope.Indexing.HostBase
{
    public class HostWrapper
    {
        public IHostBuilder DefaultBuilder;

        protected HostWrapper(IHostBuilder builder)
        {
            DefaultBuilder = builder;
        }

        public static HostWrapper Create(Action<IHostBuilder> builder)
        {
            IHostBuilder obj = new HostBuilder();
            builder(obj);

            return new HostWrapper(obj);
        }

        public virtual async Task<int> Run(string name, IConfiguration configuration)
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
                    Log.Fatal(e, "{Name} terminated unexpectedly", name);
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
