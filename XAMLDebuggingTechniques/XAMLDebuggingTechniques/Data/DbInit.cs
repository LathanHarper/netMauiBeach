using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Prism.Ioc;

namespace XAMLDebuggingTechniques.Data
{
    public static class DbInit
    {
        /// <summary>
        /// Ensures the database is created and lightly seeded—first good set of the morning.
        /// </summary>
        /// <param name="container">Prism container for resolving <see cref="IDbContextFactory{TContext}"/>.</param>
        /// <param name="ct">Cancellation token to bail out if the surf turns.</param>
        public static async Task InitializeAsync(IContainerProvider container, CancellationToken ct = default)
        {
            var factory = container.Resolve<IDbContextFactory<AppDbContext>>();
            await using var db = await factory.CreateDbContextAsync(ct);

            // Prefer migrations; fallback to EnsureCreated when none exist yet.
            var hasMigrations = db.Database.GetMigrations().Any();
            if (hasMigrations)
            {
                await db.Database.MigrateAsync(ct);
            }
            else
            {
                await db.Database.EnsureCreatedAsync(ct);
            }

            // Seed minimal data once
            if (!await db.TodoItems.AnyAsync(ct))
            {
                db.TodoItems.Add(new TodoItem { Title = "First paddle-out", Done = false });
                await db.SaveChangesAsync(ct);
            }

            // Helpful diagnostics in Debug
            try
            {
                var conn = db.Database.GetDbConnection();
                await conn.OpenAsync(ct);
                System.Diagnostics.Debug.WriteLine($"[DbInit] DataSource = {conn.DataSource}");

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND rootpage IS NOT NULL;";
                    var count = (long)(await cmd.ExecuteScalarAsync(ct) ?? 0L);
                    System.Diagnostics.Debug.WriteLine($"[DbInit] sqlite_master count = {count}");
                }
            }
            catch (Exception ex)
            {
                // No wipeouts during boot: swallow diagnostics issues but log for the crew.
                System.Diagnostics.Debug.WriteLine($"[DbInit] Diagnostics failed: {ex.Message}");
            }
        }
    }
}
