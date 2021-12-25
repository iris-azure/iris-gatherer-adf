using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Models;
using Microsoft.Extensions.Logging;

namespace IrisGathererADF.Serializers
{
  public class CosmosDbSerializer : ISerializer
  {
    private readonly ILogger<CosmosDbSerializer> _logger;

    public CosmosDbSerializer(ILogger<CosmosDbSerializer> logger)
    {
      _logger = logger;
    }

    public async Task SerializeAsync(DataFactory dataFactory, CancellationToken cancellationToken)
    {
      string jsonString = JsonSerializer.Serialize<DataFactory>(dataFactory, 
                                                                new JsonSerializerOptions() 
                                                                { 
                                                                  WriteIndented = true 
                                                                });
      _logger.LogInformation(jsonString);
    }
  }
}