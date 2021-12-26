using IrisGathererADF.Models;
using Xunit;

namespace IrisGatherer.Test
{
  public class PipelineTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.Pipeline: Blank constructor test")]
    public void Blank_Constructor_Test()
    {
      Pipeline pipeline = new Pipeline();

      Assert.True(pipeline.Id.Length == 0);
      Assert.True(pipeline.Name.Length == 0);
      Assert.True(pipeline.Description.Length == 0);
      Assert.True(pipeline.Folder.Length == 0);
      Assert.Empty(pipeline.RunStatuses);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Pipeline: Data Filled Test")]
    public void Data_Filled_Test()
    {
      Pipeline pipeline = new Pipeline()
      {
        Id = "|jdjdjdj|subscription|somevalue",
        Name = "test-pipeline",
        Description = "Some test description",
        Folder = "folder1"
      };
      pipeline.RunStatuses.Add(new RunStatus());

      Assert.True(pipeline.Id.Equals("|jdjdjdj|subscription|somevalue"));
      Assert.True(pipeline.Name.Equals("test-pipeline"));
      Assert.True(pipeline.Description.Equals("Some test description"));
      Assert.True(pipeline.Folder.Equals("folder1"));
      Assert.True(pipeline.RunStatuses.Count == 1);
    }
  }
}