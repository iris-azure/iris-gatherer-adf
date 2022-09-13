using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Serialization;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;

using Moq;
using Xunit;
using Newtonsoft.Json;

using IrisGathererADF.Serializers;
using IrisGathererADF.Gatherers;
using IrisGathererADF.Models.Config;

namespace IrisGatherer.Test
{
  public class ADFGathererTest
  {
    private IrisGathererADF.Models.DataFactory param;
    private JsonSerializerSettings DeserializationSettings;

    public ADFGathererTest()
    {
      DeserializationSettings = new JsonSerializerSettings
      {
          DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,
          DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
          NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
          ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize,
          ContractResolver = new ReadOnlyJsonContractResolver(),
          Converters = new List<JsonConverter>
              {
                  new Iso8601TimeSpanConverter()
              }
      };
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<SecretBase>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<FactoryRepoConfiguration>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<IntegrationRuntime>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<IntegrationRuntimeStatus>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<LinkedService>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Dataset>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Activity>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Trigger>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<DataFlow>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<Credential>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<WebLinkedServiceTypeProperties>("authenticationType"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<DatasetStorageFormat>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<DatasetLocation>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<DependencyReference>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<CompressionReadSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<FormatReadSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<StoreReadSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<ExportSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<CopySource>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<ImportSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<StoreWriteSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<FormatWriteSettings>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<CopySink>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<LinkedIntegrationRuntimeType>("authorizationType"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<CustomSetupBase>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<SsisObjectMetadata>("type"));
      DeserializationSettings.Converters.Add(new PolymorphicDeserializeJsonConverter<CopyTranslator>("type"));
      DeserializationSettings.Converters.Add(new TransformationJsonConverter());
      DeserializationSettings.Converters.Add(new CloudErrorJsonConverter());  }

    [Fact(DisplayName = "IrisGathererADF.Gatherers.ADFGatherer: Normal Operation")]
    public void Normal_Operation()
    {

      string data = @"
        {
          ""name"": ""exampleFactoryName"",
          ""id"": ""/subscriptions/12345678-1234-1234-1234-12345678abc/resourceGroups/exampleResourceGroup/providers/Microsoft.DataFactory/factories/exampleFactoryName"",
          ""type"": ""Microsoft.DataFactory/factories"",
          ""properties"": {
            ""provisioningState"": ""Succeeded"",
            ""createTime"": ""2018-06-19T05:41:50.0041314Z"",
            ""version"": ""2018-06-01""
          },
          ""eTag"": ""\""00004004-0000-0000-0000-5b28979e0000\"""",
          ""location"": ""East US"",
          ""tags"": {
            ""exampleTag"": ""exampleValue""
          }
        }
      ";

      AzureOperationResponse<Factory> res = new AzureOperationResponse<Factory>();
      res.Body = SafeJsonConvert.DeserializeObject<Factory>(data, DeserializationSettings);

      Mock<IFactoriesOperations> factOper = new Mock<IFactoriesOperations>();

      factOper.Setup<AzureOperationResponse<Factory>>(
        x => x.GetWithHttpMessagesAsync(It.IsAny<string>(),
                                        It.IsAny<string>(),
                                        It.IsAny<string>(),
                                        It.IsAny<Dictionary<string, List<string>>>(),
                                        It.IsAny<CancellationToken>()).Result)
                                        .Returns(res);

      Mock<IDataFactoryManagementClient> client = 
        new Mock<IDataFactoryManagementClient>();
      
      client.Setup<IFactoriesOperations>(x => x.Factories).Returns(factOper.Object);

      data = @"
      {
        ""value"": [
          {
            ""id"": ""/subscriptions/12345678-1234-1234-1234-12345678abc/resourceGroups/exampleResourceGroup/providers/Microsoft.DataFactory/factories/exampleFactoryName/pipelines/examplePipeline"",
            ""name"": ""examplePipeline"",
            ""type"": ""Microsoft.DataFactory/factories/pipelines"",
            ""properties"": {
              ""description"": ""Example description"",
              ""activities"": [],
              ""parameters"": {
                ""OutputBlobNameList"": {
                  ""type"": ""Array""
                }
              }
            },
            ""etag"": ""0a006cd4-0000-0000-0000-5b245bd60000""
          }
        ]
      }      
      ";

      AzureOperationResponse<IPage<PipelineResource>> res1 
        = new AzureOperationResponse<IPage<PipelineResource>>();

      res1.Body = SafeJsonConvert.DeserializeObject<Page<PipelineResource>>(data, DeserializationSettings);

      Mock<IPipelinesOperations> pipeOper = new Mock<IPipelinesOperations>();

      pipeOper.Setup<AzureOperationResponse<IPage<PipelineResource>>> (
        x => x.ListByFactoryWithHttpMessagesAsync(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<Dictionary<string, List<string>>>(),
          It.IsAny<CancellationToken>()
        ).Result
      ).Returns(res1);

      client.Setup<IPipelinesOperations>(x => x.Pipelines).Returns(pipeOper.Object);

      data = @"
        {
          ""value"": [
            {
              ""runId"": ""2f7fdb90-5df1-4b8e-ac2f-064cfa58202b"",
              ""pipelineName"": ""examplePipeline"",
              ""parameters"": {
                ""OutputBlobNameList"": ""[\""exampleoutput.csv\""]""
              },
              ""invokedBy"": {
                ""id"": ""80a01654a9d34ad18b3fcac5d5d76b67"",
                ""invokedByType"": ""Manual"",
                ""name"": ""AzurePipeline""
              },
              ""runStart"": ""2018-06-16T00:37:44.6257014Z"",
              ""runEnd"": ""2018-06-16T00:38:12.7314495Z"",
              ""durationInMs"": 28105,
              ""status"": ""Succeeded"",
              ""message"": """",
              ""lastUpdated"": ""2018-06-16T00:38:12.7314495Z"",
              ""annotations"": [],
              ""runDimension"": {
                ""JobId"": ""79c1cc52-265f-41a5-9553-be65e736fbd3""
              }
            },
            {
              ""runId"": ""16ac5348-ff82-4f95-a80d-638c1d47b721"",
              ""pipelineName"": ""examplePipeline"",
              ""parameters"": {
                ""OutputBlobNameList"": ""[\""exampleoutput.csv\""]""
              },
              ""invokedBy"": {
                ""id"": ""7c5fd7ef7e8a464b98b931cf15fcac66"",
                ""invokedByType"": ""Manual"",
                ""name"": ""AzurePipeline""
              },
              ""runStart"": ""2018-06-16T00:39:49.2745128Z"",
              ""runEnd"": null,
              ""durationInMs"": null,
              ""status"": ""Cancelled"",
              ""message"": """",
              ""lastUpdated"": ""2018-06-16T00:39:51.216097Z"",
              ""annotations"": [],
              ""runDimension"": {
                ""JobId"": ""84a3c493-0628-4b44-852f-ef5b3a11bdab""
              }
            }
          ]
        }      
      ";

      AzureOperationResponse<PipelineRunsQueryResponse> res2 = 
        new AzureOperationResponse<PipelineRunsQueryResponse>();
      
      res2.Body = SafeJsonConvert.DeserializeObject<PipelineRunsQueryResponse>(data, DeserializationSettings);

      Mock<IPipelineRunsOperations> pRunOper = new Mock<IPipelineRunsOperations>();

      pRunOper.Setup<AzureOperationResponse<PipelineRunsQueryResponse>>(
        x => x.QueryByFactoryWithHttpMessagesAsync(
          It.IsAny<string>(),
          It.IsAny<string>(),
          It.IsAny<RunFilterParameters>(),
          It.IsAny<Dictionary<string, List<string>>>(),
          It.IsAny<CancellationToken>()
        ).Result
      ).Returns(res2);

      client.Setup<IPipelineRunsOperations>(x => x.PipelineRuns).Returns(pRunOper.Object);

      Mock<ISerializer> serializer = new Mock<ISerializer>();

      serializer.Setup(
        x => x.InitializeAsync(It.IsAny<CancellationToken>())
      ).Verifiable();

      serializer.Setup(
        x => x.SerializeAsync(
          It.IsAny<IrisGathererADF.Models.DataFactory>(),
          It.IsAny<CancellationToken>()
        )
      ).Callback<IrisGathererADF.Models.DataFactory, CancellationToken>( (df, c) => param = df )
      .Returns(Task.CompletedTask)
      .Verifiable();

      Mock<ILogger<ADFGatherer>> logger = new Mock<ILogger<ADFGatherer>>();

      IGatherer gatherer = new ADFGatherer(logger.Object, serializer.Object, new JobParams(), client.Object);

      PipelineList pList = new PipelineList();
      pList.FactoryList.Add(
        new PipelineInfo
        {
          Name = "exampleFactory",
          ResourceGroup = "exampleRG",
          SubscriptionId = "2f7fdb90-5df1-4b8e-ac2f-064cfa58202b",
          Environment = "AzureCloud"
        }
      );

      gatherer.Gather(pList, CancellationToken.None).GetAwaiter().GetResult();

      serializer.Verify(s => s.InitializeAsync(It.IsAny<CancellationToken>()), Times.Once);
      serializer.Verify(
        s => s.SerializeAsync(
          It.IsAny<IrisGathererADF.Models.DataFactory>(), 
          It.IsAny<CancellationToken>()
        ), 
        Times.Once);

      Assert.True(param.Id.Equals("|subscriptions|12345678-1234-1234-1234-12345678abc|resourceGroups|exampleResourceGroup|providers|Microsoft.DataFactory|factories|exampleFactoryName"));
      Assert.True(param.Name.Equals("exampleFactoryName"));
      Assert.True(param.Location.Equals("East US"));
      Assert.True(param.ResourceGroup.Equals("exampleRG"));
      Assert.True(param.Version.Equals("2018-06-01"));
      Assert.True(param.Environment.Equals("AzureCloud"));

      Assert.True(param.Pipelines[0].Id.Equals("|subscriptions|12345678-1234-1234-1234-12345678abc|resourceGroups|exampleResourceGroup|providers|Microsoft.DataFactory|factories|exampleFactoryName|pipelines|examplePipeline"));
      Assert.True(param.Pipelines[0].Name.Equals("examplePipeline"));
      Assert.True(string.IsNullOrEmpty(param.Pipelines[0].Folder));
      Assert.True(param.Pipelines[0].Description.Equals("Example description"));

      Assert.True(param.Pipelines[0].RunStatuses[0].RunId.Equals("2f7fdb90-5df1-4b8e-ac2f-064cfa58202b"));
      Assert.Null(param.Pipelines[0].RunStatuses[0].RunGroupId);
      Assert.True(param.Pipelines[0].RunStatuses[0].RunStart.Equals((new DateTime(2018, 06, 16, 0, 37, 44, 625)).AddTicks(7014)));
      Assert.True(param.Pipelines[0].RunStatuses[0].RunEnd.Equals((new DateTime(2018, 06, 16, 0, 38, 12, 731)).AddTicks(4495)));
      Assert.True(param.Pipelines[0].RunStatuses[0].DurationInMs == 28105);
      Assert.True(param.Pipelines[0].RunStatuses[0].Status.Equals("Succeeded"));
      Assert.True(param.Pipelines[0].RunStatuses[0].LastUpdated.Equals((new DateTime(2018, 06, 16, 00, 38, 12, 731)).AddTicks(4495)));
      Assert.True(param.Pipelines[0].RunStatuses[0].InvokedById.Equals("80a01654a9d34ad18b3fcac5d5d76b67"));
      Assert.True(param.Pipelines[0].RunStatuses[0].InvokedByName.Equals("AzurePipeline"));
      Assert.True(param.Pipelines[0].RunStatuses[0].InvokedByType.Equals("Manual"));

      Assert.True(param.Pipelines[0].RunStatuses[1].RunId.Equals("16ac5348-ff82-4f95-a80d-638c1d47b721"));
      Assert.Null(param.Pipelines[0].RunStatuses[1].RunGroupId);
      Assert.True(param.Pipelines[0].RunStatuses[1].RunStart.Equals((new DateTime(2018, 06, 16, 0, 39, 49, 274)).AddTicks(5128)));
      Assert.Null(param.Pipelines[0].RunStatuses[1].RunEnd);
      Assert.Null(param.Pipelines[0].RunStatuses[1].DurationInMs);
      Assert.True(param.Pipelines[0].RunStatuses[1].Status.Equals("Cancelled"));
      Assert.True(param.Pipelines[0].RunStatuses[1].LastUpdated.Equals((new DateTime(2018, 06, 16, 00, 39, 51, 216)).AddTicks(970)));
      Assert.True(param.Pipelines[0].RunStatuses[1].InvokedById.Equals("7c5fd7ef7e8a464b98b931cf15fcac66"));
      Assert.True(param.Pipelines[0].RunStatuses[1].InvokedByName.Equals("AzurePipeline"));
      Assert.True(param.Pipelines[0].RunStatuses[1].InvokedByType.Equals("Manual"));
    }  
  }
}