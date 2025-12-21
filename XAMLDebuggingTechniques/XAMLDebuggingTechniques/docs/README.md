# XAMLDebuggingTechniques – Template Guide
Your Milage may vary...
Small, fast, and salted for real surf: .NET MAUI + Prism MVVM + EF Core (SQLite + SQLCipher), hardened startup, trimming-safe, and ready for migrations.

## 1) Stack Overview

- UI: .NET MAUI (XAML) with Prism MVVM
- Navigation & DI: Prism (DryIoc under the hood)
- Data: EF Core + SQLitePCLRaw.bundle_e_sqlcipher (SQLCipher)
- Encryption: Interceptor applies `PRAGMA key` on open
- Db lifetime: `IDbContextFactory<AppDbContext>` for safe, scoped contexts
- Hardening: platform exception funnels + XAML BindingDiagnostics (DEBUG)
- Orchestration: `ApplicationLifetime.AppOrchestrator` (lifecycle + fault intake)
- Trimming + Perf: Compiled EF model (optional) + trimmer descriptors

## 2) Startup Flow (composition only)

csharp
// MauiProgram.cs (trimmed)
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");
var migrationsAssembly = typeof(AppDbContext).Assembly.FullName!;

builder.Services.AddSingleton<EncryptionKeyProvider>();
builder.Services.AddSingleton<IEncryptionKeyProvider>(sp => sp.GetRequiredService<EncryptionKeyProvider>());
builder.Services.AddSingleton<IEncryptionKeyWarmup>(sp => sp.GetRequiredService<EncryptionKeyProvider>());
builder.Services.AddSingleton<SqliteEncryptionInterceptor>();

builder.Services.AddDbContextFactory<AppDbContext>((sp, opts) =>
{
    opts.UseSqlite($"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Private",
                   b => b.MigrationsAssembly(migrationsAssembly));
    opts.AddInterceptors(sp.GetRequiredService<SqliteEncryptionInterceptor>());

    // Prefer compiled model if generated (dotnet ef dbcontext optimize)
    var compiledModelType = Type.GetType("XAMLDebuggingTechniques.Data.CompiledModels.AppDbContextModel, XAMLDebuggingTechniques");
    var instance = compiledModelType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
    if (instance is IModel model) opts.UseModel(model);
});

// Convenience scoped context
builder.Services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

.UsePrism(prism =>
{
    prism.OnInitialized(async c =>
    {
        await DbInit.InitializeAsync(c);                // migrations-first
        var warm = Application.Current.GetService<IEncryptionKeyWarmup>();
        if (warm is not null) await warm.WarmUpAsync(); // key cache
        var orch = c.Resolve<IAppOrchestrator>();
        await orch.OnStartAsync();
    });
    prism.RegisterTypes(c =>
    {
        c.RegisterSingleton<IAppOrchestrator, AppOrchestrator>();
        c.RegisterSingleton<IDbPathProvider, DbPathProvider>();
        c.RegisterForNavigation<NavigationPage>();
        c.RegisterForNavigation<MainPage, MainPageViewModel>();
    })
    .CreateWindow("NavigationPage/MainPage");
});

## 3) Platform Hardening

- Culture defaults (`en-US`) set early
- Global exception nets (AppDomain + TaskScheduler)
- Android adds `AndroidEnvironment.UnhandledExceptionRaiser`
- XAML `BindingDiagnostics.BindingFailed` (DEBUG only)
- All funnel into `IAppOrchestrator` via `Application.Current.GetService<T>()`

csharp
// Windows example (App.xaml.cs)
private static void UnhandledExceptionHandler(object? s, UnhandledExceptionEventArgs e)
{
    var ex = e.ExceptionObject as Exception;
    Application.Current.GetService<IAppOrchestrator>()?.ReportUnhandledException(ex!, "Windows AppDomain");
}

## 4) Database & Encryption

- SQLCipher via native bundle; interceptor applies key and sets WAL
- Key provider (`IEncryptionKeyProvider`) currently demo-only (replace before ship)
- Warmup interface (`IEncryptionKeyWarmup`) to pre-cache the key
- Rekey support via `IKeyRotationService` (`PRAGMA rekey`)

csharp
// Apply key on open (SqliteEncryptionInterceptor)
cmd.CommandText = $"PRAGMA key = '{escaped}';";
await cmd.ExecuteNonQueryAsync(ct);
cmd.CommandText = "PRAGMA journal_mode = wal;";
await cmd.ExecuteNonQueryAsync(ct);

## 5) Migrations-first Initialization

- Prefers `MigrateAsync()` when any migrations exist
- Falls back to `EnsureCreatedAsync()` otherwise

csharp
// DbInit.InitializeAsync
var hasMigrations = db.Database.GetMigrations().Any();
if (hasMigrations) await db.Database.MigrateAsync(ct);
else await db.Database.EnsureCreatedAsync(ct);

## 6) Trimming + Performance

- Compiled EF model (generate once and commit):

bash
# From repo root
 dotnet ef dbcontext optimize \
   -p XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj \
   -s XAMLDebuggingTechniques/XAMLDebuggingTechniques.csproj \
   -o Data/CompiledModels

- Trimmer descriptor keeps DbContext + entities safe:

xml
<linker>
  <assembly fullname="XAMLDebuggingTechniques">
    <type fullname="XAMLDebuggingTechniques.Data.AppDbContext" preserve="all" />
    <type fullname="XAMLDebuggingTechniques.Data.TodoItem" preserve="all" />
  </assembly>
</linker>

- Optional attribute nudge for the trimmer:

csharp
// AppDbContext constructor
[DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TodoItem))]
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

## 7) Orchestrator (ApplicationLifetime)

- Single point for lifetime + faults:
  - OnStartAsync / OnResumeAsync / OnSleepAsync / OnExitAsync
  - ReportUnhandledException / ReportUnobservedTaskException / ReportBindingFailure
- Resolved via DI in VMs and through platform hosts at boot

csharp
public interface IAppOrchestrator
{
    Task OnStartAsync(CancellationToken ct = default);
    Task OnResumeAsync(CancellationToken ct = default);
    Task OnSleepAsync(CancellationToken ct = default);
    Task OnExitAsync(bool dueToCrash, CancellationToken ct = default);
    void ReportUnhandledException(Exception ex, string channel, string? context = null);
    void ReportUnobservedTaskException(AggregateException ex, string channel, string? context = null);
    void ReportBindingFailure(string? property, Uri? source, int? line, string message);
}

## 8) ViewModel Commands

- `SeedCommand`: add a todo
- `ReadCommand`: query and bind
- `VerifyEncryptionCommand`: checks cipher, integrity, header
- `WipeAndRecreateEncryptedCommand`: DEV-ONLY reset

csharp
// MainPageViewModel (excerpt)
public async Task<IEnumerable<TodoItem>> LoadAsync()
{
    await using var db = await _dbFactory.CreateDbContextAsync();
    return await db.TodoItems.OrderBy(x => x.Done).ThenBy(x => x.Id).ToListAsync();
}

## 9) Debugging Experience

- DebuggerDisplay on DbContext, ViewModel, entity classes
- `ApplicationExtensions.GetService<T>` marked `[DebuggerStepThrough]`
- BindingDiagnostics under DEBUG with source info (file/line)

## 10) Migrations Cheatsheet

See `docs/ef-migrations.md` for CLI commands and runtime preferences.

## 11) Template Token Ideas

- AppId, DisplayName
- Db file name (default: app.db)
- Default Culture (default: en-US)
- Initial route (NavigationPage/MainPage)
- Use compiled EF model (true/false)
- Key provider strategy (demo/secure)

## 12) Ship Checklist

- Replace demo key strategy (IEncryptionKeyProvider)
- Generate compiled EF model and commit
- Verify trimming on Android/iOS real devices
- Add telemetry sink (ILogger provider) if desired
- Add additional entities to trimmer descriptor or rely fully on compiled model
- Validate migrations-first logic with real migrations

Paddle out: `dotnet build`, deploy to Android/iOS, and you’re riding clean. 
