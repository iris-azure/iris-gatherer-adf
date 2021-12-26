using IrisGathererADF.Models.Config;
using Xunit;

namespace IrisGatherer.Test
{
  public class PipelineInfoTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.Config.PipelineInfo: Blank constructor test")]
    public void Blank_Constructor_Test()
    {
      PipelineInfo pipelineInfo = new PipelineInfo();

      Assert.True(pipelineInfo.Name.Length == 0);
      Assert.True(pipelineInfo.ResourceGroup.Length == 0);
      Assert.True(pipelineInfo.Environment.Length == 0);
      Assert.True(pipelineInfo.SubscriptionId.Length == 0);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Config.PipelineInfo: Data Filled Test")]
    public void Data_Filled_Test()
    {
      PipelineInfo pipelineInfo = new PipelineInfo()
      {
        Environment = "dev",
        ResourceGroup = "test-rg",
        Name = "test-adf",
        SubscriptionId = "90ecb49f-f4b3-4da7-87db-4a459b33a5dd"
      };

      Assert.Equal("dev", pipelineInfo.Environment);
      Assert.Equal("test-rg", pipelineInfo.ResourceGroup);
      Assert.Equal("test-adf", pipelineInfo.Name);
      Assert.Equal("90ecb49f-f4b3-4da7-87db-4a459b33a5dd", pipelineInfo.SubscriptionId);
    }
  }
}