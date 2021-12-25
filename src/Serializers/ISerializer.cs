using System.Threading;
using System.Threading.Tasks;
using IrisGathererADF.Models;

namespace IrisGathererADF.Serializers
{
  public interface ISerializer
  {
    Task SerializeAsync(DataFactory dataFactory, CancellationToken cancellationToken);
  }
}