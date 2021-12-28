using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace IrisGathererADF.Models.Config
{
  public class PipelineList
  {
    private static HttpClient httpClient;

    [JsonPropertyName(name: "factorylist")]
    public List<PipelineInfo> FactoryList { get; set; }

    static PipelineList() => httpClient = new HttpClient();

    public PipelineList()
    {
      FactoryList = new List<PipelineInfo>();
    }

    public static async Task<PipelineList> Initialize(JobParams jobParams,
                                                      CancellationToken cancellationToken,
                                                      HttpMessageHandler handler = null)
    {
      PipelineList list = null;

      if (handler is not null)
      {
        httpClient = new HttpClient(handler);
      }

      if (jobParams is null)
      {
        throw new ArgumentNullException("Job Parameters is null");
      }

      switch(jobParams.ListLocation)
      {
        case "storage":
          list = await InitializeFromStorage(jobParams, cancellationToken);
          break;
        default:
          throw new ArgumentException("Unsupported list location", "ListLocation");
      }
      
      return list;
    }

    private static async Task<PipelineList> InitializeFromStorage(JobParams jobParams, CancellationToken cancellationToken)
    {
      using Stream openStream = await httpClient.GetStreamAsync(jobParams.ListStorageURL, cancellationToken);
      PipelineList list = await JsonSerializer.DeserializeAsync<PipelineList>(openStream, cancellationToken: cancellationToken);

      return list;
    }
  }
}