using System;
using IrisGathererADF.Models;
using Xunit;

namespace IrisGatherer.Test
{
  public class RunStatusTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.RunStatus: Blank constructor test")]
    public void Blank_Constructor_Test()
    {
      RunStatus status = new RunStatus();

      Assert.True(status.RunId.Length == 0);
      Assert.True(status.RunGroupId.Length == 0);
      Assert.Null(status.RunStart);
      Assert.Null(status.RunEnd);
      Assert.True(status.Status.Length == 0);
      Assert.True(status.PipelineName.Length == 0);
      Assert.Null(status.LastUpdated);
      Assert.True(status.InvokedById.Length == 0);
      Assert.True(status.InvokedByName.Length == 0);
      Assert.True(status.InvokedByType.Length == 0);
      Assert.Null(status.DurationInMs);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.RunStatus: Data Filled Test")]
    public void Data_Filled_Test()
    {
      RunStatus status = new RunStatus()
      {
        RunId = "90ecb49f-f4b3-4da7-87db-4a459b33a5dd",
        RunGroupId = "90ecb49f-f4b3-4da7-87db-4a459b33a5dd",
        RunStart = new DateTime(2021, 01, 01, 12, 12, 12),
        RunEnd = new DateTime(2021, 01, 01, 12, 12, 12),
        Status = "Success",
        PipelineName = "test-pipeline",
        LastUpdated = new DateTime(2021, 01, 01, 12, 12, 12),
        InvokedById = "90ecb49f-f4b3-4da7-87db-4a459b33a5dd",
        InvokedByName = "test-name",
        InvokedByType = "test-user",
        DurationInMs = 1000
      };

      Assert.True(status.RunId.Equals("90ecb49f-f4b3-4da7-87db-4a459b33a5dd"));
      Assert.True(status.RunGroupId.Equals("90ecb49f-f4b3-4da7-87db-4a459b33a5dd"));
      Assert.Equal(new DateTime(2021, 01, 01, 12, 12, 12), status.RunStart);
      Assert.Equal(new DateTime(2021, 01, 01, 12, 12, 12), status.RunEnd);
      Assert.Equal(new DateTime(2021, 01, 01, 12, 12, 12), status.LastUpdated);
      Assert.True(status.Status.Equals("Success"));
      Assert.True(status.PipelineName.Equals("test-pipeline"));
      Assert.True(status.InvokedById.Equals("90ecb49f-f4b3-4da7-87db-4a459b33a5dd"));
      Assert.True(status.InvokedByName.Equals("test-name"));
      Assert.True(status.InvokedByType.Equals("test-user"));
      Assert.Equal(1000, status.DurationInMs);
    }

  }
}