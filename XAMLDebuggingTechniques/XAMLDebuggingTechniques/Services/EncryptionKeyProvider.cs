using Microsoft.Maui.Storage;
using System.Diagnostics;

namespace XAMLDebuggingTechniques.Services
{
    [DebuggerDisplay("EncryptionKeyProvider")]
    public sealed class EncryptionKeyProvider : IEncryptionKeyProvider, IEncryptionKeyWarmup
    {
        private const string KeyName = "db_key";
        private string? _cached;

        /// <summary>
        /// Retrieves the SQLCipher passphrase. Demo setup keeps it simple; for real surf,
        /// derive per-device or store in secure platform keystore.
        /// </summary>
        public async Task WarmUpAsync(CancellationToken ct = default)
        {
            // Populate cache early to avoid any sync-over-async cost later.
            _ = GetKey();
            await Task.CompletedTask;
        }

        public string GetKey()
        {
            if (!string.IsNullOrEmpty(_cached)) return _cached;

            // Demo only: persist or derive securely. Replace before shipping.
            var existing = SecureStorage.GetAsync(KeyName).GetAwaiter().GetResult();
            if (!string.IsNullOrEmpty(existing))
                return _cached = existing;

            // Generate a simple passphrase; prefer a stronger derivation in production.
            var generated = "change-this-passphrase";
            SecureStorage.SetAsync(KeyName, generated).GetAwaiter().GetResult();
            return _cached = generated;
        }
    }
}