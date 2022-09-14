using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace IrisGathererADF.Models
{
  public class Pipeline
  {
    [JsonProperty("id")]
    [JsonPropertyName(name: "id")]
    public string Id { get; set; }

    [JsonProperty("name")]
    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonProperty("desc")]
    [JsonPropertyName(name: "desc")]
    public string Description { get; set; }

    [JsonProperty("folder")]
    [JsonPropertyName(name: "folder")]
    public string Folder { get; set; }

    [JsonProperty("runStatuses")]
    [JsonPropertyName(name: "runStatuses")]
    public List<RunStatus> RunStatuses { get; set; }

    public Pipeline()
    {
      Id = string.Empty;
      Name = string.Empty;
      Description = string.Empty;
      Folder = string.Empty;
      RunStatuses = new List<RunStatus>();
    }
  }
}