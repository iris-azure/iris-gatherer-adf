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
    private PipelineList? _list;

    public GathererJob(ILogger<GathererJob> logger, IGatherer gatherer, JobParams jobParams)
    {
      _logger = logger;
      _gatherer = gatherer;
      _jobParams = jobParams;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Bootstrapping the job...");

      _logger.LogInformation("Initializing factory list...");
      _list = PipelineList.Initialize(_jobParams, cancellationToken).Result;

      _logger.LogInformation("Setting timer...");
      _timer = new Timer(DoWork,
                         _list,
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

      if (state == null)
      {
        _logger.LogError("Error: Factory list not received!");
      }
      else
      {
        PipelineList list = (PipelineList) state;

        _logger.LogInformation(list.FactoryList[0].ResourceGroup);
      }
    }

    public void Dispose()
    {
      _logger.LogInformation("Job cleanup started...");
      _timer?.Dispose();
      _logger.LogInformation("Job cleanup complete.");
    }
  }
}