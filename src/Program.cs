using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IrisGathererADF.Jobs;
using IrisGathererADF.Gatherers;

namespace IrisGathererADF
{
  class Program
  {
    public static async Task Main(string[] args)
    {
      using var host = CreateHostBuilder(args).Build();
      await host.StartAsync();
      await host.WaitForShutdownAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureHostConfiguration((config) =>
          {
            config.AddEnvironmentVariables(prefix: "IRISAZ_GATHERER_");
          })
          .ConfigureServices((hostContext, services) =>
          {
            services.AddTransient<IGatherer, ADFGatherer>();
            services.AddHostedService<GathererJob>();
          });
  }
}

