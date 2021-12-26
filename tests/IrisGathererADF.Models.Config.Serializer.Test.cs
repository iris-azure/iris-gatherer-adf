using IrisGathererADF.Models.Config;
using Xunit;

namespace IrisGatherer.Test
{
  public class SerializerTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.Config.Serializer: Blank constructor test")]
    public void Blank_Constructor_Test()
    {
      Serializer serializer = new Serializer();

      Assert.True(serializer.Type.Length == 0);
      Assert.True(serializer.ConnectionString.Length == 0);
      Assert.True(serializer.Container.Length == 0);
      Assert.True(serializer.DatabaseName.Length == 0);
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Config.Serializer: Data Filled Test")]
    public void Data_Filled_Test()
    {
      Serializer serializer = new Serializer()
      {
        Type = "cosmos",
        ConnectionString = "connectionstring",
        Container = "container",
        DatabaseName = "database name"
      };

      Assert.Equal("cosmos", serializer.Type);
      Assert.Equal("connectionstring", serializer.ConnectionString);
      Assert.Equal("container", serializer.Container);
      Assert.Equal("database name", serializer.DatabaseName);
    }
  }
}