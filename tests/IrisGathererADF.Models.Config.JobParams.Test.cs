using System;
using IrisGathererADF.Models.Config;
using Xunit;

namespace IrisGatherer.Test
{
  public class JobParamsTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.Config.JobParams: Blank constructor test")]
    public void Blank_Constructor_Test()
    {
      JobParams jparams = new JobParams();

      Assert.True(jparams.ListLocation.Length == 0);
      Assert.True(jparams.ListStorageURL.Length == 0);
      Assert.Equal(10, jparams.DaysToKeep);
      Assert.Equal(600, jparams.TriggerPeriodSeconds);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Config.JobParams: Data Filled Test")]
    public void Data_Filled_Test()
    {
      JobParams jobParams = new JobParams()
      {
        ListLocation = "storage",
        ListStorageURL = "http://example.com",
        DaysToKeep = 20,
        TriggerPeriodSeconds = 450
      };

      Assert.Equal("storage", jobParams.ListLocation);
      Assert.Equal("http://example.com", jobParams.ListStorageURL);
      Assert.Equal(20, jobParams.DaysToKeep);
      Assert.Equal(450, jobParams.TriggerPeriodSeconds);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Config.JobParams: Illegal trigger period test")]
    public void Illegal_Trigger_Period_Test()
    {
      Assert.Throws<ArgumentException>
      (
        () => new JobParams() { TriggerPeriodSeconds = 299 }
      );
    }
  }
}