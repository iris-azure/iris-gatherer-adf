using System;
using IrisGathererADF.Models;
using Xunit;

namespace IrisGatherer.Test
{
  public class DataFactoryTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.DataFactory: Blank constructor test")]
    public void Blank_Constructor_Test()
    {
      DataFactory adf = new DataFactory();

      Assert.True(adf.Id.Length == 0);
      Assert.True(adf.Environment.Length == 0);
      Assert.True(adf.Location.Length == 0);
      Assert.True(adf.Name.Length == 0);
      Assert.True(adf.ResourceGroup.Length == 0);
      Assert.True(adf.Version.Length == 0);
      Assert.True(adf.Pipelines.Count == 0);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.DataFactory: Data Filled Test")]
    public void Data_Filled_Test()
    {
      DataFactory adf = new DataFactory()
      {
        Id = "TestID|ID1|XXX",
        Environment = "prod",
        Location = "centralus",
        Name = "test-environment-adf",
        ResourceGroup = "test-environment-rg",
        Version = "2.0"
      };
      adf.Pipelines.Add(new Pipeline());

      Assert.True(adf.Id.Equals("TestID|ID1|XXX"));
      Assert.True(adf.Environment.Equals("prod"));
      Assert.True(adf.Location.Equals("centralus"));
      Assert.True(adf.Name.Equals("test-environment-adf"));
      Assert.True(adf.ResourceGroup.Equals("test-environment-rg"));
      Assert.True(adf.Version.Equals("2.0"));
      Assert.True(adf.Pipelines.Count == 1);
    }
  }
}
