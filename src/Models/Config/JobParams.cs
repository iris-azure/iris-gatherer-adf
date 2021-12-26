using System;

namespace IrisGathererADF.Models.Config
{
  public class JobParams
  {
    private long _triggerPeriodSeconds;

    public long TriggerPeriodSeconds 
    { 
      get { return _triggerPeriodSeconds; } 
      set
      {
        if (value < 300)
        {
          throw new ArgumentException("Triggering period is too low. Minimum 300 seconds required",
                                      "TriggerPeriodSeconds");
        }

        _triggerPeriodSeconds = value;
      } 
    }

    public string ListLocation { get; set; }

    public string ListStorageURL { get; set; }

    public int DaysToKeep { get; set; }

    public JobParams()
    {
      TriggerPeriodSeconds = 600;
      ListLocation = string.Empty;
      ListStorageURL = string.Empty;
      DaysToKeep = 10;
    }
  }
}