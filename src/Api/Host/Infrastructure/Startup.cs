using Cinder.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cinder.Api.Host.Infrastructure
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
            services.AddCors(options =>
            {
                string[] origins = Configuration.GetSection("Cors:Origins").Get<string[]>();
                options.AddDefaultPolicy(builder => builder.WithOrigins(origins).WithMethods("GET"));
            });
            services.AddOptions(Configuration);
            services.AddDatabase();
            services.AddMediator();
            services.AddSearch();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddValidation();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler("/Error");
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
