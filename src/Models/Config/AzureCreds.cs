

namespace IrisGathererADF.Models.Config
{
  public class AzureCreds
  {
    public string AppId { get; set; }
    public string Secret { get; set; }
    public string SubscriptionId { get; set; }

    public AzureCreds()
    {
      AppId = string.Empty;
      Secret = string.Empty;
      SubscriptionId = string.Empty;
    }
  }
}