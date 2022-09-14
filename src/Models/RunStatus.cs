using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace IrisGathererADF.Models
{
  public class RunStatus
  {
    [JsonProperty("runGroupId")]
    [JsonPropertyName(name: "runGroupId")]
    public string RunGroupId { get; set; }

    [JsonProperty("runId")]
    [JsonPropertyName(name: "runId")]
    public string RunId { get; set; }

    [JsonProperty("runStart")]
    [JsonPropertyName(name: "runStart")]
    public DateTime? RunStart { get; set; }

    [JsonProperty("runEnd")]
    [JsonPropertyName(name: "runEnd")]
    public DateTime? RunEnd { get; set; }

    [JsonProperty("durationInMs")]
    [JsonPropertyName(name: "durationInMs")]
    public int? DurationInMs { get; set; }

    [JsonProperty("pipelineName")]
    [JsonPropertyName(name: "pipelineName")]
    public string PipelineName { get; set; }

    [JsonProperty("invokedByType")]
    [JsonPropertyName(name: "invokedByType")]
    public string InvokedByType { get; set; }

    [JsonProperty("invokedByName")]
    [JsonPropertyName(name: "invokedByName")]
    public string InvokedByName { get; set; }

    [JsonProperty("invokedById")]
    [JsonPropertyName(name: "invokedById")]
    public string InvokedById { get; set; }

    [JsonProperty("lastUpdated")]
    [JsonPropertyName(name: "lastUpdated")]
    public DateTime? LastUpdated { get; set; }

    [JsonProperty("status")]
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
      InvokedById = string.Empty;
      InvokedByType = string.Empty;
      InvokedByName = string.Empty;
      InvokedByType = string.Empty;
      LastUpdated = null;
      Status = string.Empty;
    }
  }
}