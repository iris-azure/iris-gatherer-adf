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
    private static Task? _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

    static GathererJob()
    {
      _executingTask = null;
    }

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
                         null,
                         TimeSpan.Zero,
                         TimeSpan.FromSeconds(_jobParams.TriggerPeriodSeconds));
      _logger.LogInformation("Job bootstrap complete.");
      return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Stopping the job...");

      _timer?.Change(Timeout.Infinite, 0);

      if (_executingTask != null)
      {
        try
        {
          _stoppingCts.Cancel();
        }
        finally
        {
          await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
      }

      _logger.LogInformation("Job stopped.");

      return;
    }

    private void DoWork(object? state)
    {
      _logger.LogInformation("DoWork triggered...");

      if (_executingTask == null || 
          _executingTask.Status == TaskStatus.RanToCompletion ||
          _executingTask.Status == TaskStatus.Faulted)
      {
        _executingTask = new Task(() => _gatherer.Gather(_list, _stoppingCts.Token));
        _executingTask.Start();

        _logger.LogInformation("DoWork: Gatherer started.");
      }
      else
      {
        _logger.LogWarning("A task is already running, hence skipping. Consider increasing the interval.");
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