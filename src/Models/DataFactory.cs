using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IrisGathererADF.Models
{
  public class DataFactory
  {
    [JsonPropertyName(name: "id")]
    public string Id { get; set; }

    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonPropertyName(name: "location")]
    public string Location { get; set; }

    [JsonPropertyName(name: "rg")]
    public string ResourceGroup { get; set; }

    [JsonPropertyName(name: "ver")]
    public string Version { get; set; }

    [JsonPropertyName(name: "pipelines")]
    public List<Pipeline> Pipelines { get; set; }

    public DataFactory()
    {
      Id = string.Empty;
      Name = string.Empty;
      Location = string.Empty;
      ResourceGroup = string.Empty;
      Version = string.Empty;
      Pipelines = new List<Pipeline>();
    }
  }
}