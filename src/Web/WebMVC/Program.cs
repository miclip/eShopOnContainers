using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Microsoft.eShopOnContainers.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
             .AddCommandLine(args)
             .AddEnvironmentVariables(prefix: "ASPNETCORE_")
             .Build();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseIISIntegration()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
