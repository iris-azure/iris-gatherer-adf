using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using IrisGathererADF.Models;
using IrisGathererADF.Models.Config;
using IrisGathererADF.Serializers;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace IrisGathererADF.Gatherers
{
  public class ADFGatherer : IGatherer
  {
    private readonly ILogger<ADFGatherer> _logger;
    private readonly ISerializer _serializer;
    private readonly DefaultAzureCredential _defCreds;
    private readonly AzureCreds _creds;

    private readonly JobParams _jobParams;

    public ADFGatherer(ILogger<ADFGatherer> logger,
                       ISerializer serializer,
                       AzureCreds creds,
                       DefaultAzureCredential defCreds,
                       JobParams jobParams)
    {
      _logger = logger;
      _defCreds = defCreds;
      _creds = creds;
      _serializer = serializer;
      _jobParams = jobParams;
    }

    public async Task Gather(PipelineList pipelineList,
                             CancellationToken cancellationToken)
    {
      try
      {
        _logger.LogInformation("Gather: Initializing Serializer");
        await _serializer.InitializeAsync(cancellationToken);
        
        _logger.LogInformation("Gather: Starting to gather");
        foreach(PipelineInfo pipelineInfo in pipelineList.FactoryList)
        {
          if (cancellationToken.IsCancellationRequested)
          {
            _logger.LogInformation("Gather: Cancellation requested. Hence exiting.");
            break;
          }

          await GatherForAFactory(pipelineInfo, cancellationToken);
        }
        _logger.LogInformation("Gather: Gather complete");        
      }
      catch (System.Exception ex)
      {
        _logger.LogError(ex, "Error Occured. Look at exception details.");
        throw;
      }
    }

    private async Task GatherForAFactory(PipelineInfo pipelineInfo,
                                         CancellationToken cancellationToken)
    {
      // Initialize ADF client
      string token = _defCreds.GetToken(
        new Azure.Core.TokenRequestContext(
          new string[]{"https://management.azure.com/"})).Token;
          
      ServiceClientCredentials cred = new TokenCredentials(token);
      DataFactoryManagementClient client = new DataFactoryManagementClient(cred)
      {
        SubscriptionId = pipelineInfo.SubscriptionId
      };
      
      _logger.LogInformation($"GatherForAFactory: Gathering Data factory details for {pipelineInfo.Name} in {pipelineInfo.ResourceGroup}");
      Factory factory = await client.Factories.GetAsync(pipelineInfo.ResourceGroup,
                                                        pipelineInfo.Name,
                                                        null,
                                                        cancellationToken);
      DataFactory adf = new DataFactory();
      adf.Id = factory.Id.Replace("/", "|");
      adf.Name = factory.Name;
      adf.Location = factory.Location;
      adf.ResourceGroup = pipelineInfo.ResourceGroup;
      adf.Version = factory.Version;
      adf.Environment = pipelineInfo.Environment;

      Pipeline pipeline;
      RunStatus pipelineStatus;

      _logger.LogInformation($"GatherForAFactory: Listing pipelines for {pipelineInfo.Name} in {pipelineInfo.ResourceGroup}");
      var pipelines = await client.Pipelines.ListByFactoryAsync(pipelineInfo.ResourceGroup, 
                                                                pipelineInfo.Name, 
                                                                cancellationToken);
      foreach(PipelineResource pipeline_i in pipelines)
      {
        _logger.LogInformation($"GatherForAFactory:     Adding details for {pipeline_i.Name}");
        pipeline = new Pipeline();
        pipeline.Id = pipeline_i.Id.Replace("/", "|");
        pipeline.Name = pipeline_i.Name;
        pipeline.Folder = (pipeline_i.Folder == null ? string.Empty : pipeline_i.Folder.Name);
        pipeline.Description = pipeline_i.Description;

        var pipelineruns = await client.PipelineRuns.QueryByFactoryAsync(
                                    pipelineInfo.ResourceGroup, 
                                    pipelineInfo.Name, 
                                    new RunFilterParameters(
                                      DateTime.UtcNow.AddHours(-24 * _jobParams.DaysToKeep),
                                      DateTime.UtcNow,
                                      null,
                                      new List<RunQueryFilter> 
                                      {
                                        new RunQueryFilter("PipelineName", 
                                                          "Equals", 
                                                          new List<string> { pipeline.Name })
                                      },
                                      new List<RunQueryOrderBy>()), 
                                    cancellationToken);

        foreach(PipelineRun prun in pipelineruns.Value)
        {
          pipelineStatus = new RunStatus();
          pipelineStatus.RunId = prun.RunId;
          pipelineStatus.RunGroupId = prun.RunGroupId;
          pipelineStatus.RunStart = prun.RunStart;
          pipelineStatus.RunEnd = prun.RunEnd;
          pipelineStatus.DurationInMs = prun.DurationInMs;
          pipelineStatus.Status = prun.Status;
          pipelineStatus.LastUpdated = prun.LastUpdated;
          pipelineStatus.InvokedById = prun.InvokedBy.Id;
          pipelineStatus.InvokedByName = prun.InvokedBy.Name;
          pipelineStatus.InvokedByType = prun.InvokedBy.InvokedByType;

          pipeline.RunStatuses.Add(pipelineStatus);
        }

        adf.Pipelines.Add(pipeline);
      }
      
      _logger.LogInformation($"Saving details for {pipelineInfo.Name} in {pipelineInfo.ResourceGroup}");
      await _serializer.SerializeAsync(adf, cancellationToken);

      return;
    }
  }
}