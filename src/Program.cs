using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IrisGathererADF.Jobs;
using IrisGathererADF.Gatherers;
using IrisGathererADF.Models.Config;
using IrisGathererADF.Serializers;
using Azure.Identity;

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
            IConfiguration config = hostContext.Configuration;

            AzureCreds creds = new AzureCreds();
            JobParams jobParams = new JobParams();

            config.GetSection("AzureCreds").Bind(creds);
            config.GetSection("JobParams").Bind(jobParams);

            services.AddSingleton(creds);
            services.AddSingleton(jobParams);
            services.AddTransient<IGatherer, ADFGatherer>();
            services.AddHostedService<GathererJob>();

            switch(config.GetValue<string>("Serializer"))
            {
              case "cosmos":
                services.AddTransient<ISerializer, CosmosDbSerializer>();
                break;
              default:
                throw new ArgumentException("Invalid serializer specified.", "Serializer");
            }

            DefaultAzureCredentialOptions credOpts = new DefaultAzureCredentialOptions
            {
              ExcludeAzureCliCredential = false,
              ExcludeAzurePowerShellCredential = true,
              ExcludeEnvironmentCredential = false,
              ExcludeInteractiveBrowserCredential = true,
              ExcludeManagedIdentityCredential = true,
              ExcludeSharedTokenCacheCredential = true,
              ExcludeVisualStudioCodeCredential = true,
              ExcludeVisualStudioCredential = true
            };

            DefaultAzureCredential defCred = new DefaultAzureCredential(credOpts);
            services.AddSingleton(creds);
          });
  }
}

