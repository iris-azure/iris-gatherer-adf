using System.Text.Json.Serialization;

namespace IrisGathererADF.Models
{
  public class Pipeline
  {
    [JsonPropertyName(name: "id")]
    public string Id { get; set; }

    [JsonPropertyName(name: "name")]
    public string Name { get; set; }

    [JsonPropertyName(name: "desc")]
    public string Description { get; set; }

    [JsonPropertyName(name: "folder")]
    public string Folder { get; set; }

    public Pipeline()
    {
      Id = string.Empty;
      Name = string.Empty;
      Description = string.Empty;
      Folder = string.Empty;
    }
  }
}