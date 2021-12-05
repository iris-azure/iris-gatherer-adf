using System;
using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Gatherers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

#nullable enable

namespace IrisGathererADF.Jobs
{
  public class GathererJob : IHostedService, IDisposable
  {
    private readonly ILogger<GathererJob> _logger;
    private readonly IGatherer _gatherer;
    private Timer? _timer;

    public GathererJob(ILogger<GathererJob> logger, IGatherer gatherer)
    {
      _logger = logger;
      _gatherer = gatherer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Inside StartAsync.");
      _timer = new Timer(DoWork,
                         null,
                         TimeSpan.Zero,
                         TimeSpan.FromSeconds(5));
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Inside StopAsync.");
      _timer?.Change(Timeout.Infinite, 0);
      return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
      _logger.LogInformation("Inside DoWork");
    }

    public void Dispose()
    {
      _logger.LogInformation("Inside Dispose.");
      _timer?.Dispose();
    }
  }
}