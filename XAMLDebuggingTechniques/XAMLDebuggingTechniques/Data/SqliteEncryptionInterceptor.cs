using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using XAMLDebuggingTechniques.Services;

namespace XAMLDebuggingTechniques.Data
{
    /// <summary>
    /// EF Core connection interceptor that applies the SQLCipher key and journaling settings
    /// as soon as a SQLite connection opens—no unkeyed queries slip past the shore break.
    /// </summary>
    [DebuggerDisplay("SqliteEncryptionInterceptor")] 
    public sealed class SqliteEncryptionInterceptor : DbConnectionInterceptor
    {
        private readonly IEncryptionKeyProvider _keyProvider;

        /// <summary>
        /// Create a new interceptor that can fetch the encryption key.
        /// </summary>
        public SqliteEncryptionInterceptor(IEncryptionKeyProvider keyProvider)
            => _keyProvider = keyProvider;

        public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
        {
            await ApplyKeyAsync(connection, cancellationToken).ConfigureAwait(false);
            await base.ConnectionOpenedAsync(connection, eventData, cancellationToken).ConfigureAwait(false);
        }

        public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
        {
            ApplyKeyAsync(connection, CancellationToken.None).GetAwaiter().GetResult();
            base.ConnectionOpened(connection, eventData);
        }

        /// <summary>
        /// Applies the `PRAGMA key` to an open <see cref="SqliteConnection"/> and configures WAL.
        /// </summary>
        private async Task ApplyKeyAsync(DbConnection connection, CancellationToken ct)
        {
            if (connection is not SqliteConnection sqlite)
                return;

            if (sqlite.State != System.Data.ConnectionState.Open)
                return; // Only apply key to an open connection

            var key = _keyProvider.GetKey();
            var escaped = key.Replace("'", "''"); // escape single quotes

            await using var cmd = sqlite.CreateCommand();
            cmd.CommandText = $"PRAGMA key = '{escaped}';";
            await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);

            // It's safe to set WAL after key, so journaling operates on encrypted pages
            cmd.CommandText = "PRAGMA journal_mode = wal;";
            try { await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false); } catch { /* best effort */ }
        }

    }
}