using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace IrisGathererADF.Models
{
  public class DataFactory
  {
    [JsonProperty("id")]
    [JsonPropertyName(name: "id")]
    public string Id { get; set; }

    [JsonProperty("env")]
    [JsonPropertyName(name: "env")]
    public string Environment { get; set; }
    
    [JsonProperty("name")]
    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonProperty("location")]
    [JsonPropertyName(name: "location")]
    public string Location { get; set; }

    [JsonProperty("rg")]
    [JsonPropertyName(name: "rg")]
    public string ResourceGroup { get; set; }

    [JsonProperty("ver")]
    [JsonPropertyName(name: "ver")]
    public string Version { get; set; }

    [JsonProperty("pipelines")]
    [JsonPropertyName(name: "pipelines")]
    public List<Pipeline> Pipelines { get; set; }

    public DataFactory()
    {
      Id = string.Empty;
      Environment = string.Empty;
      Name = string.Empty;
      Location = string.Empty;
      ResourceGroup = string.Empty;
      Version = string.Empty;
      Pipelines = new List<Pipeline>();
    }
  }
}