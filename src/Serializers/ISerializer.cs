using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Models;

namespace IrisGathererADF.Serializers
{
  public interface ISerializer
  {
    Task InitializeAsync(CancellationToken cancellationToken);
    Task SerializeAsync(DataFactory dataFactory, CancellationToken cancellationToken);
  }
}