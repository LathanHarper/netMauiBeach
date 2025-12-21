using XAMLDebuggingTechniques.Data;
// DbInit.cs
using Microsoft.EntityFrameworkCore;
using System.Text;
using XAMLDebuggingTechniques.Services;

using System;
using System.Collections.Generic;

namespace XAMLDebuggingTechniques.Data
{
    public static class DbInitDry
    {
        public static async Task InitializeAsync(IContainerProvider sp, CancellationToken ct = default)
        {
            var factory = sp.Resolve<IDbContextFactory<AppDbContext>>();
            await using var db = await factory.CreateDbContextAsync(ct);

            // Day-1 minimal (no migrations yet):
            await db.Database.EnsureCreatedAsync(ct);

            // Seed (runs once)
            if (!await db.TodoItems.AnyAsync(ct))
            {
                db.TodoItems.AddRange(
                    new TodoItem { Title = "Paddle out", Done = false },
                    new TodoItem { Title = "Slide a cheater-five", Done = false }
                );
                await db.SaveChangesAsync(ct);
            }
        }
    }
}

public static class DbInit
{
    // Prism container version
    public static async Task InitializeAsync(IContainerProvider sp, CancellationToken ct = default)
    {
        // (Optional) log the path we expect; your builder uses FileSystem.AppDataDirectory + "app.db"
        var expectedPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
        System.Diagnostics.Debug.WriteLine($"[DbInit] FileSystem.AppDataDirectory = {FileSystem.AppDataDirectory}");
        System.Diagnostics.Debug.WriteLine($"[DbInit] Expected DB path            = {expectedPath}");

        // List files so we can see the .db actually exists after migration
        try
        {
            var files = Directory.GetFiles(FileSystem.AppDataDirectory);
            System.Diagnostics.Debug.WriteLine($"[DbInit] AppDataDirectory files ({files.Length}):");
            foreach (var f in files) System.Diagnostics.Debug.WriteLine($"[DbInit]  - {f}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DbInit] Could not list AppDataDirectory: {ex}");
        }

        // Resolve the factory registered via AddDbContextFactory in MauiProgram
        var factory = sp.Resolve<IDbContextFactory<AppDbContext>>();
        await using var db = await factory.CreateDbContextAsync(ct);

        // Try migrations first (prod), fall back to EnsureCreated for first-install safety
        try
        {
            var pending = await db.Database.GetPendingMigrationsAsync(ct);
            if (pending.Any())
                await db.Database.MigrateAsync(ct);
            else
                await db.Database.EnsureCreatedAsync(ct);
        }
        catch (InvalidOperationException mex) when (
            mex.Message.Contains("PendingModelChangesWarning"))
        {
            System.Diagnostics.Debug.WriteLine("[DbInit] Pending model changes; using EnsureCreated.");
            await db.Database.EnsureCreatedAsync(ct);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DbInit] Migration failed: {ex}");
            System.Diagnostics.Debug.WriteLine("[DbInit] Falling back to EnsureCreated.");
           await db.Database.EnsureCreatedAsync(ct);
        }
        {
            //#if WINDOWS
            //#else
           await db.Database.EnsureCreatedAsync(ct);
            //#endif
           return;
        }
        
        // Introspect the ACTUAL sqlite file opened by this connection
        try
        {
            var conn = db.Database.GetDbConnection();
            await conn.OpenAsync(ct);
            System.Diagnostics.Debug.WriteLine($"[DbInit] Actual SQLite DataSource = {conn.DataSource}");

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "PRAGMA database_list;";
                using var rdr = await cmd.ExecuteReaderAsync(ct);
                while (await rdr.ReadAsync(ct))
                {
                    // seq, name, file
                    System.Diagnostics.Debug.WriteLine($"[DbInit] PRAGMA database_list => name={rdr.GetString(1)}, file={rdr.GetString(2)}");
                }
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
                using var rdr = await cmd.ExecuteReaderAsync(ct);
                var sb = new StringBuilder("[DbInit] sqlite_master tables:");
                while (await rdr.ReadAsync(ct))
                    sb.Append("\n  - ").Append(rdr.GetString(0));
                System.Diagnostics.Debug.WriteLine(sb.ToString());
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DbInit] Introspection failed: {ex}");
        }

        // Also confirm EF’s model knows about TodoItems
        try
        {
            var tables = db.Model.GetEntityTypes().Select(t => t.GetTableName()).Distinct().ToList();
            System.Diagnostics.Debug.WriteLine("[DbInit] EF model tables: " + string.Join(", ", tables));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DbInit] Model introspection failed: {ex}");
        }

        // Seed once
        try
        {
            if (!await db.TodoItems.AnyAsync(ct))
        {
            db.TodoItems.Add(new TodoItem { Title = "First paddle-out", Done = false });
            await db.SaveChangesAsync(ct);
                System.Diagnostics.Debug.WriteLine("[DbInit] Seed inserted 1 TodoItem.");
            }

            var count = await db.TodoItems.CountAsync(ct);
            System.Diagnostics.Debug.WriteLine($"[DbInit] TodoItems.Count() = {count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DbInit] SEED/READ failed: {ex}");
            }
    }
}


