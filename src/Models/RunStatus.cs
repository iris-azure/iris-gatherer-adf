using System;
using System.Text.Json.Serialization;

namespace IrisGathererADF.Models
{
  public class RunStatus
  {
    [JsonPropertyName(name: "runGroupId")]
    public string RunGroupId { get; set; }

    [JsonPropertyName(name: "runId")]
    public string RunId { get; set; }

    [JsonPropertyName(name: "runStart")]
    public DateTime? RunStart { get; set; }

    [JsonPropertyName(name: "runEnd")]
    public DateTime? RunEnd { get; set; }

    [JsonPropertyName(name: "durationInMs")]
    public int? DurationInMs { get; set; }

    [JsonPropertyName(name: "pipelineName")]
    public string PipelineName { get; set; }

    [JsonPropertyName(name: "invokedByType")]
    public string InvokedByType { get; set; }

    [JsonPropertyName(name: "invokedByName")]
    public string InvokedByName { get; set; }

    [JsonPropertyName(name: "invokedById")]
    public string InvokedById { get; set; }

    [JsonPropertyName(name: "lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [JsonPropertyName(name: "status")]
    public string Status { get; set; }

    public RunStatus()
    {
      RunGroupId = string.Empty;
      RunId = string.Empty;
      RunStart = null;
      RunEnd = null;
      DurationInMs = null;
      PipelineName = string.Empty;
      InvokedByType = string.Empty;
      InvokedByName = string.Empty;
      InvokedByType = string.Empty;
      LastUpdated = null;
      Status = string.Empty;
    }
  }
}