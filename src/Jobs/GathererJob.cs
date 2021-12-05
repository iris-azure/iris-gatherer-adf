using System;
using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Gatherers;
using IrisGathererADF.Models.Config;
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
    private JobParams _jobParams;

    public GathererJob(ILogger<GathererJob> logger, IGatherer gatherer, JobParams jobParams)
    {
      _logger = logger;
      _gatherer = gatherer;
      _jobParams = jobParams;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Bootstrapping the job...");
      _timer = new Timer(DoWork,
                         null,
                         TimeSpan.Zero,
                         TimeSpan.FromSeconds(_jobParams.TriggerPeriodSeconds));
      _logger.LogInformation("Job bootstrap complete.");
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Stopping the job...");
      _timer?.Change(Timeout.Infinite, 0);
      _logger.LogInformation("Job stopped.");
      return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
      _logger.LogInformation("Inside DoWork");
    }

    public void Dispose()
    {
      _logger.LogInformation("Job cleanup started...");
      _timer?.Dispose();
      _logger.LogInformation("Job cleanup complete.");
    }
  }
}