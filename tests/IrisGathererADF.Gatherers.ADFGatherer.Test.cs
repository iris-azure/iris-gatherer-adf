using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Moq;
using Xunit;

namespace IrisGatherer.Test
{
  public class ADFGathererTest
  {
    [Fact(DisplayName = "IrisGathererADF.Gatherers.ADFGatherer: Normal Operation")]
    public void Normal_Operation()
    {
      Factory factory = new Factory(id: "/subscription/resourcegroup/factories/factory",
                                    name: "");
      AzureOperationResponse<Factory> res = new AzureOperationResponse<Factory>();
      res.Body = factory;

      Mock<IFactoriesOperations> factOper = new Mock<IFactoriesOperations>();

      // factOper.Setup<Factory>(x => x.GetAsync(It.IsAny<string>(),
      //                                         It.IsAny<string>(),
      //                                         It.IsAny<string>(),
      //                                         It.IsAny<CancellationToken>()).Result)
      //                                         .Returns(factory);

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
    }  
  }
}