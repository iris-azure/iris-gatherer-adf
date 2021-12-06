using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Models.Config;

namespace IrisGathererADF.Gatherers
{
  public interface IGatherer
  {
    Task Gather(PipelineList pipelineList, CancellationToken cancellationToken);
  }

}