using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IrisGathererADF
{
  class Program
  {
    public static ServiceProvider serviceProvider;

    static int Main(string[] args)
    {
      var services = ConfigureServices();
      serviceProvider = services.BuildServiceProvider();

      // var cts = new CancellationTokenSource();
      // var token = cts.Token;
      // var serviceBus = serviceProvider.GetService<IServiceBusWorker>();

      // AppDomain.CurrentDomain.ProcessExit += (s, e) => { cts.Cancel(); };
      // serviceBus.ReceiveMessages();
      // Thread.Sleep(Timeout.Infinite);
      return 0;
    }

    private static IServiceCollection ConfigureServices()
    {
      IServiceCollection services = new ServiceCollection();

      // Set up the objects we need to get to configuration settings
      var config = LoadConfiguration();

      // Add Logging
      services.AddLogging(logging =>
      {
        logging.AddConfiguration(config.GetSection("Logging"));
        logging.AddSystemdConsole(options =>
                {
                  options.IncludeScopes = true;
                });
      });
      // Add the config to our DI container for later user
      services.AddSingleton(config);

      // Register interfaces with implementation

      return services;
    }

    private static IConfiguration LoadConfiguration()
    {
      var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddEnvironmentVariables(prefix: Consts.EnvVarPrefix);
      return builder.Build();
    }
  }
}

