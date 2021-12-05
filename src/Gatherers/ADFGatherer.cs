using Microsoft.Extensions.Logging;

namespace IrisGathererADF.Gatherers
{
  public class ADFGatherer : IGatherer
  {
    private readonly ILogger<ADFGatherer> _logger;

    public ADFGatherer(ILogger<ADFGatherer> logger)
    {
      _logger = logger;
    }
  }
}