using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace XAMLDebuggingTechniques.Services
{
    public sealed class KeyRotationService : IKeyRotationService
    {
        private readonly IDbContextFactory<Data.AppDbContext> _factory;
        private readonly IEncryptionKeyProvider _keys;

        public KeyRotationService(IDbContextFactory<Data.AppDbContext> factory, IEncryptionKeyProvider keys)
        {
            _factory = factory;
            _keys = keys;
        }

        public async Task RekeyAsync(string newKey, CancellationToken ct = default)
        {
            await using var db = await _factory.CreateDbContextAsync(ct);
            await db.Database.OpenConnectionAsync(ct);
            var conn = (SqliteConnection)db.Database.GetDbConnection();
            await using var cmd = conn.CreateCommand();
            var escaped = newKey.Replace("'", "''");
            cmd.CommandText = $"PRAGMA rekey = '{escaped}';";
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}
