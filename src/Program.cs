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
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Rest;

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

            JobParams jobParams = new JobParams();
            Serializer serializer = new Serializer();

            config.GetSection("JobParams").Bind(jobParams);
            config.GetSection("Serializer").Bind(serializer);

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
            services.AddSingleton(defCred);
            services.AddSingleton(jobParams);
            services.AddSingleton(serializer);
            services.AddTransient<IGatherer, ADFGatherer>();
            services.AddTransient<IDataFactoryManagementClient>((serviceProvider) => 
            {
              DefaultAzureCredential creds = serviceProvider.GetRequiredService<DefaultAzureCredential>();
              string token = creds.GetToken(
                new Azure.Core.TokenRequestContext(
                  new string[]{"https://management.azure.com/"})).Token;
              
              DataFactoryManagementClient client = new DataFactoryManagementClient(new TokenCredentials(token));
              return client;
            });
            services.AddHostedService<GathererJob>();

            switch(serializer.Type)
            {
              case "cosmos":
                services.AddTransient<ISerializer, CosmosDbSerializer>();
                break;
              default:
                throw new ArgumentException("Invalid serializer specified.", "Serializer");
            }

          });
  }
}

