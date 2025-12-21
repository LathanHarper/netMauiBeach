# EF Core + SQLite (SQLCipher) Migrations Cheatsheet

> Project targets .NET MAUI (multi-TFM). Run tooling from the solution root.

## Add a migration

```bash
# Use the main MAUI project as both the startup and target project
# Adjust names as needed
 dotnet ef migrations add InitialCreate \
   -p XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj \
   -s XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj
```

## Remove last migration (if not applied)
```bash
 dotnet ef migrations remove \
   -p XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj \
   -s XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj
```

## List migrations
```bash
 dotnet ef migrations list \
   -p XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj \
   -s XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj
```

## Apply migrations at runtime

`DbInit.InitializeAsync` prefers `MigrateAsync()` when migrations exist; otherwise it falls back to `EnsureCreatedAsync()`.

```csharp
await using var db = await factory.CreateDbContextAsync(ct);
var hasMigrations = (await db.Database.GetMigrationsAsync(ct)).Any();
if (hasMigrations)
{
    await db.Database.MigrateAsync(ct);
}
else
{
    await db.Database.EnsureCreatedAsync(ct);
}
```

## Notes
- The project uses `SQLitePCLRaw.bundle_e_sqlcipher` and an EF Core interceptor to apply `PRAGMA key`.
- Prefer a private page cache (Cache=Private) and disable pooling when doing key validation.
