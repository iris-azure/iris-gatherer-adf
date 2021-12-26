using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Cosmos;
using IrisGathererADF.Models;
using IrisGathererADF.Models.Config;
using Microsoft.Extensions.Logging;

namespace IrisGathererADF.Serializers
{
  public class CosmosDbSerializer : ISerializer
  {
    private readonly ILogger<CosmosDbSerializer> _logger;
    private readonly Serializer _config;
    private CosmosClient _client;
    private CosmosDatabase _db;
    private CosmosContainer _container;

    public CosmosDbSerializer(ILogger<CosmosDbSerializer> logger, Serializer config)
    {
      _logger = logger;
      _config = config;

      _client = new CosmosClient(_config.ConnectionString);
      _db = null;
      _container = null;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
      _db = await _client.CreateDatabaseIfNotExistsAsync(id: _config.DatabaseName, cancellationToken: cancellationToken);
      _container = await _db.CreateContainerIfNotExistsAsync(id: _config.Container,
                                                             partitionKeyPath: "/env",
                                                             cancellationToken: cancellationToken);
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