namespace ShoppingListService.WebApi
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Could be cleaner/better when ASP.NET Core supports easier bootstrapping
            AssemblyLoadContext.Default.Resolving += (context, name) =>
                {
                    if (name.Name.EndsWith("resources"))
                    {
                        return null;
                    }

                    var entryPath = Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName;
                    var assemblyPath = Directory.GetFiles(entryPath, name.Name + ".dll", SearchOption.AllDirectories).FirstOrDefault();
                    return assemblyPath != null ? context.LoadFromAssemblyPath(assemblyPath) : null;
                };

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", true)
                .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
