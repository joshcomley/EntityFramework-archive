using EfSample9.Hazception.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using WebApplication2.Hazception;

namespace WebApplication2
{
    public class Startup
    {
        public static string ConnectionString = "Server=.;Database=Hazception.App.Data;Integrated Security=True;";

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEntityFrameworkSqlServer();
            services.AddOData();
            services.AddTransient<HazceptionQueryFilterOptions>();
            services.AddTransient<ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(
                o => o.UseSqlServer(ConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
            IEdmModel model = ODataConfigurator.ConfigureHazception(app);

            // Single
            app.UseMvc(builder => builder.MapODataRoute("odata", model));

            // Multiple: Option-1
            //app.UseMvc(builder =>
            //{
            //    builder.MapODataRoute("a", model);
            //});

            //// Multiple: Option-2
            //app.UseMvc(builder => builder.MapODataRoute("odata1", model));
        }
    }
}
