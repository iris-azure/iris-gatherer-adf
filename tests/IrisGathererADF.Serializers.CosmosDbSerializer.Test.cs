using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Cosmos;
using IrisGathererADF.Models;
using IrisGathererADF.Models.Config;
using IrisGathererADF.Serializers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IrisGatherer.Test
{
  public class CosmosDbSerializerTest
  {
    [Fact(DisplayName = "IrisGathererADF.Serializers.CosmosDbSerializer: Serialize without initialization test")]
    public void Serialize_Without_Initialization_Test()
    {
      Mock<ILogger<CosmosDbSerializer>> logger = new Mock<ILogger<CosmosDbSerializer>>();
      Serializer config = new Serializer()
      {
        ConnectionString = "AccountEndpoint=https://localhost:443/;AccountKey=badbad==;"
      };

      ISerializer serializer = new CosmosDbSerializer(logger.Object, config);

      Assert.ThrowsAsync<InvalidOperationException>(async () =>
      {
        await serializer.SerializeAsync(new DataFactory(), CancellationToken.None);
      });
    }

    [Fact(DisplayName = "IrisGathererADF.Serializers.CosmosDbSerializer: Normal operation test")]
    public void Normal_Operation_Test()
    {
      Mock<ItemResponse<DataFactory>> itemResponse = new Mock<ItemResponse<DataFactory>>();

      Mock<CosmosContainer> container = new Mock<CosmosContainer>();
      container.Setup<ItemResponse<DataFactory>>(x => x.UpsertItemAsync<DataFactory>(
                                              It.IsAny<DataFactory>(),
                                              It.IsAny<PartitionKey?>(),
                                              It.IsAny<ItemRequestOptions>(),
                                              It.IsAny<CancellationToken>()).Result)
                                              .Returns(itemResponse.Object);
      Mock<ContainerResponse> response = new Mock<ContainerResponse>();
      response.Setup(x => x.Container).Returns(container.Object);
      
      Mock<CosmosDatabase> database = new Mock<CosmosDatabase>();
      database.Setup<ContainerResponse>(x => x.CreateContainerIfNotExistsAsync( 
                                              It.IsAny<string>(),
                                              It.IsAny<string>(),
                                              It.IsAny<int?>(),
                                              It.IsAny<RequestOptions>(),
                                              It.IsAny<CancellationToken>()).Result)
                                              .Returns(response.Object);
      Mock<DatabaseResponse> dbResponse = new Mock<DatabaseResponse>();
      dbResponse.Setup(x => x.Database).Returns(database.Object);

      Mock<CosmosClient> client = new Mock<CosmosClient>();
      client.Setup<DatabaseResponse>(x => x.CreateDatabaseIfNotExistsAsync(
                                              It.IsAny<string>(),
                                              It.IsAny<int?>(),
                                              It.IsAny<RequestOptions>(),
                                              It.IsAny<CancellationToken>()).Result)
                                              .Returns(dbResponse.Object);
    
      Mock<ILogger<CosmosDbSerializer>> logger = new Mock<ILogger<CosmosDbSerializer>>();
      Serializer config = new Serializer()
      {
        DatabaseName     = "test1",
        Container        = "container1",
        ConnectionString = "AccountEndpoint=https://localhost:443/;AccountKey=badbad==;"
      };

      ISerializer serializer = new CosmosDbSerializer(logger.Object, config, client.Object);

      serializer.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
      
      Exception ex;

      ex = Record.Exception( () => 
            client.Verify<Task<DatabaseResponse>>(x => 
                    x.CreateDatabaseIfNotExistsAsync(config.DatabaseName, 
                                                     null, null, CancellationToken.None), 
                                                     Times.Once));
      Assert.Null(ex);

      ex = null;
      ex = Record.Exception( () => 
            database.Verify<Task<ContainerResponse>>(x => 
                    x.CreateContainerIfNotExistsAsync(config.Container, "/env",
                                                     null, null, CancellationToken.None), 
                                                     Times.Once));
      Assert.Null(ex);

DataFactory adf = new DataFactory()
      {
        Id = "TestID|ID1|XXX",
        Environment = "prod",
        Location = "centralus",
        Name = "test-environment-adf",
        ResourceGroup = "test-environment-rg",
        Version = "2.0"
      };
      adf.Pipelines.Add(new Pipeline()
      {
        Id = "TestID|ID1|XXX|pipelines|test",
        Description = "Test description",
        Folder = "test",
        Name = "pipeline1"
      });
      adf.Pipelines[0].RunStatuses.Add(new RunStatus()
      {
        RunId = "deecd523-0930-4124-819b-3a61adb9c924",
        RunGroupId = "deecd523-0930-4124-819b-3a61adb9c924",
        DurationInMs = 1000,
        InvokedById = "deecd523-0930-4124-819b-3a61adb9c924",
        InvokedByName = "test",
        InvokedByType = "Manual",
        LastUpdated = new DateTime(2021, 12, 29, 10, 01, 05, DateTimeKind.Utc),
        PipelineName = "pipeline1",
        RunStart = new DateTime(2021, 12, 30, 00, 01, 21, DateTimeKind.Utc),
        RunEnd = new DateTime(2021, 12, 30, 00, 05, 13, DateTimeKind.Utc)
      });

      serializer.SerializeAsync(adf, CancellationToken.None).GetAwaiter().GetResult();

      ex = null;
      ex = Record.Exception( () => 
            container.Verify<Task<ItemResponse<DataFactory>>>(x => 
                    x.UpsertItemAsync<DataFactory>(adf, new PartitionKey(adf.Environment),
                                                     null, CancellationToken.None), 
                                                     Times.Once));
      Assert.Null(ex);
    }
  }
}