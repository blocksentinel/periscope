using Cinder.Extensions.Builder;
using Cinder.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cinder.Api.Infrastructure
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
            services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
            services.AddCors(options =>
            {
                string[] origins = Configuration.GetSection("Cors:Origins").Get<string[]>();
                options.AddDefaultPolicy(builder => builder.WithOrigins(origins).WithMethods("GET"));
            });
            services.AddErrorHandling();
            services.AddOptions(Configuration);
            services.AddBlockchain();
            services.AddDatabase();
            services.AddHosting();
            //services.AddMessaging();
            services.AddMediator();
            services.AddServices();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddValidation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors();
            app.UseErrorHandling();
            //app.UseMessaging();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
