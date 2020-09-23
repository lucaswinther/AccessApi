using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Access.Auth.Service.Host
{
    public class Program
    {
        public static void Main(string[] args)
            => BuildWebHost(args).Run();

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();
    }
}
