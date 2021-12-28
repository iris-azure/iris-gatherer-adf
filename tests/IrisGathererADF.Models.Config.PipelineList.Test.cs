using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Models.Config;
using Moq;
using Moq.Protected;
using Xunit;

namespace IrisGatherer.Test
{
  public class PipelineListTest
  {
    [Fact(DisplayName = "IrisGathererADF.Models.Config.PipelineList: Null Job Parameter test")]
    public void Null_Job_Parameter_Test()
    {
      Assert.ThrowsAsync<ArgumentNullException>(async () => 
      {
        PipelineList list = await PipelineList.Initialize(null, CancellationToken.None); 
      });
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Config.PipelineList: Wrong List Location test")]
    public void Wrong_List_Location_Test()
    {
      JobParams jobParams = new JobParams()
      {
        ListLocation = "garbage",
        ListStorageURL = "http://nowhere.com",
        DaysToKeep = 10,
        TriggerPeriodSeconds = 600
      };

      Assert.ThrowsAsync<ArgumentException>(async () =>
      {
        PipelineList list = await PipelineList.Initialize(jobParams, CancellationToken.None);
      });
    }

    [Fact(DisplayName = "IrisGathererADF.Models.Config.PipelineList: Normal operation test")]
    public void Normal_Operation_Test()
    {
      JobParams jobParams = new JobParams()
      {
        ListLocation = "storage",
        ListStorageURL = "http://somestorage.com",
        DaysToKeep = 10,
        TriggerPeriodSeconds = 600
      };

      string jsonString = "{\"factorylist\":[{\"name\":\"factory1-adf\",\"rg\":\"factory1-rg\",\"subscriptionId\":\"badbad45-50d3-4130-a111-762e78a6fbad\",\"env\":\"qa\"},{\"name\":\"factory2-adf\",\"rg\":\"factory2-rg\",\"subscriptionId\":\"badbad45-50d3-4130-a111-762e78a6fbad\",\"env\":\"qa\"}]}";
      HttpResponseMessage responseMessage = new HttpResponseMessage();
      responseMessage.StatusCode = System.Net.HttpStatusCode.OK;
      responseMessage.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

      Mock<HttpMessageHandler> handler = new Mock<HttpMessageHandler>();
      handler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
                                                           ItExpr.IsAny<HttpRequestMessage>(),
                                                           ItExpr.IsAny<CancellationToken>())
                                                           .ReturnsAsync(responseMessage);
      
      PipelineList list = PipelineList.Initialize(jobParams,
                                                  CancellationToken.None,
                                                  handler.Object).Result;

      Assert.Equal(2, list.FactoryList.Count);
      Assert.Equal("factory1-adf", list.FactoryList[0].Name);
      Assert.Equal("factory1-rg", list.FactoryList[0].ResourceGroup);
      Assert.Equal("badbad45-50d3-4130-a111-762e78a6fbad", list.FactoryList[0].SubscriptionId);
      Assert.Equal("qa", list.FactoryList[0].Environment);
      Assert.Equal("factory2-adf", list.FactoryList[1].Name);
      Assert.Equal("factory2-rg", list.FactoryList[1].ResourceGroup);
      Assert.Equal("badbad45-50d3-4130-a111-762e78a6fbad", list.FactoryList[1].SubscriptionId);
      Assert.Equal("qa", list.FactoryList[1].Environment);
    }
  }
}