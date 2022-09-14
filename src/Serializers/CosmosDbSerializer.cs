using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using IrisGathererADF.Models;
using IrisGathererADF.Models.Config;

namespace IrisGathererADF.Serializers
{
  public class CosmosDbSerializer : ISerializer
  {
    private readonly ILogger<CosmosDbSerializer> _logger;
    private readonly Serializer _config;
    private CosmosClient _client;
    private Database _db;
    private Container _container;

    public CosmosDbSerializer(ILogger<CosmosDbSerializer> logger,
                              Serializer config,
                              CosmosClient client = null)
    {
      _logger = logger;
      _config = config;

      if (client is null)
        _client = new CosmosClient(
          _config.ConnectionString,
          new CosmosClientOptions 
          { 
            SerializerOptions = new CosmosSerializationOptions 
            { 
              PropertyNamingPolicy = CosmosPropertyNamingPolicy.Default 
            }
          }
        );
      else
        _client = client;
        
      _db = null;
      _container = null;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
      _db = await _client.CreateDatabaseIfNotExistsAsync(id: _config.DatabaseName, 
                                                         cancellationToken: cancellationToken);
      ContainerResponse response = 
        await _db.CreateContainerIfNotExistsAsync(id: _config.Container,
                                                  partitionKeyPath: "/env",
                                                  cancellationToken: cancellationToken);
      _container = response.Container;
    }

    public async Task SerializeAsync(DataFactory dataFactory, CancellationToken cancellationToken)
    {
      if (_db == null)
      {
        _logger.LogError("SerializeAsync: Serializer not initialized.");
        throw new System.InvalidOperationException("SerializeAsync: Serializer not initialized.");
      }

      await _container.UpsertItemAsync<DataFactory>(item: dataFactory,
                                                    partitionKey: new PartitionKey(dataFactory.Environment),
                                                    cancellationToken: cancellationToken);

      _logger.LogInformation("SerializeAsync: Saved record");
    }
  }
}