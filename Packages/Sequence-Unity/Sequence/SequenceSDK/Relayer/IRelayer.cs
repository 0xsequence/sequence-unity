using System.Threading.Tasks;

namespace Sequence.Relayer
{
    public interface IRelayer
    {
        Task<FeeOptionsReturn> GetFeeOptions(FeeOptionsArgs args);
    }
}