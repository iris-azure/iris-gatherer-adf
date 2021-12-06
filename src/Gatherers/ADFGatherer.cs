using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Models.Config;
using IrisGathererADF.Serializers;
using Microsoft.Extensions.Logging;

namespace IrisGathererADF.Gatherers
{
  public class ADFGatherer : IGatherer
  {
    private readonly ILogger<ADFGatherer> _logger;
    private readonly ISerializer _serializer;

    public ADFGatherer(ILogger<ADFGatherer> logger, ISerializer serializer)
    {
      _logger = logger;
    }

    public async Task Gather(PipelineList pipelineList, CancellationToken cancellationToken)
    {
      foreach(PipelineInfo pipelineInfo in pipelineList.FactoryList)
      {
        if (cancellationToken.IsCancellationRequested)
        {
          _logger.LogInformation("Cancellation requested. Hence exiting.");
          break;
        }

        await GatherForAFactory(pipelineInfo, cancellationToken);
      }
    }

    private async Task GatherForAFactory(PipelineInfo pipelineInfo, CancellationToken cancellationToken)
    {
      _logger.LogInformation($"Listing pipelines for {pipelineInfo.Name} in {pipelineInfo.ResourceGroup}");
      Thread.Sleep(14000);
    }
  }
}