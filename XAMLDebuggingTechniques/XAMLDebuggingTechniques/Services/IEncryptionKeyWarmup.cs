using System.Threading;
using System.Threading.Tasks;

namespace XAMLDebuggingTechniques.Services
{
    public interface IEncryptionKeyWarmup
    {
        Task WarmUpAsync(CancellationToken ct = default);
    }
}
