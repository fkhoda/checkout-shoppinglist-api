namespace ShoppingListService.WebApi
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Versioning;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using ShoppingListService.WebApi.Controllers;
    using ShoppingListService.WebApi.Middleware;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile("hosting.json", true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddApiVersioning(
                options =>
                    {
                        options.ReportApiVersions = true;
                        options.AssumeDefaultVersionWhenUnspecified = true;
                        options.ApiVersionReader = new MediaTypeApiVersionReader();
                        options.DefaultApiVersion = new ApiVersion(1, 0);
                        options.Conventions.Controller<ShoppingListsController>().HasApiVersion(new ApiVersion(1, 0));
                    });

            // Could be cleaner/better when ASP.NET Core supports easier bootstrapping
            var entryPath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
            var assemblyPath = Directory.GetFiles(entryPath, "*.DependencyResolution.dll", SearchOption.AllDirectories).FirstOrDefault();
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            var solutionName = assembly.FullName.Substring(0, assembly.FullName.IndexOf('.'));
            dynamic obj = assembly.CreateInstance($"{solutionName}.DependencyResolution.DependencyRegistrar");
            var serviceProvider = services.BuildServiceProvider();
            obj.Register(serviceProvider, services, Configuration);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMiddleware<AuthenticationMiddleware>();

            app.UseMvc();
        }
    }
}
