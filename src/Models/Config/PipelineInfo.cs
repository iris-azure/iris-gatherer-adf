using System.Text.Json.Serialization;

namespace IrisGathererADF.Models.Config
{
  public class PipelineInfo
  {
    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonPropertyName(name: "rg")]
    public string ResourceGroup { get; set; }

    [JsonPropertyName(name: "subscriptionId")]
    public string SubscriptionId { get; set; }

    [JsonPropertyName(name: "env")]
    public string Environment { get; set; }

    public PipelineInfo()
    {
      Name = string.Empty;
      ResourceGroup = string.Empty;
      SubscriptionId = string.Empty;
      Environment = string.Empty;
    }

  }
}