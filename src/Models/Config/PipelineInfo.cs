using System.Text.Json.Serialization;

namespace IrisGathererADF.Models.Config
{
  public class PipelineInfo
  {
    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonPropertyName(name: "rg")]
    public string ResourceGroup { get; set; }

    public PipelineInfo()
    {
      Name = string.Empty;
      ResourceGroup = string.Empty;
    }

  }
}