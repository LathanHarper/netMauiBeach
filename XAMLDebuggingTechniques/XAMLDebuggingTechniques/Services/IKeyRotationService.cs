using System.Threading;
using System.Threading.Tasks;

namespace XAMLDebuggingTechniques.Services
{
    public interface IKeyRotationService
    {
        Task RekeyAsync(string newKey, CancellationToken ct = default);
    }
}
