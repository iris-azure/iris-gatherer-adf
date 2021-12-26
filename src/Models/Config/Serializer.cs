

namespace IrisGathererADF.Models.Config
{
  public class Serializer
  {
    public string Type { get; set; }

    public string ConnectionString { get; set; }

    public string DatabaseName { get; set; }

    public string Container { get; set; }

    public Serializer()
    {
      Type = string.Empty;
      ConnectionString = string.Empty;
      DatabaseName = string.Empty;
      Container = string.Empty;
    }
  }
}