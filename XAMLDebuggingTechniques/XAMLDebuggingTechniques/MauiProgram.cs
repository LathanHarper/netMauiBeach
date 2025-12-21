using XAMLDebuggingTechniques.ViewModels;
using XAMLDebuggingTechniques.Views;
using Microsoft.Extensions.Logging;
using DryIoc;
using Prism;
using Prism.Ioc;
using Prism.Navigation;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using XAMLDebuggingTechniques.Services;
using XAMLDebuggingTechniques.Data;
using ApplicationLifetime;
using Prism.AppModel;
using XAMLDebuggingTechniques.Infrastructure;
using System;

namespace XAMLDebuggingTechniques
{
    /// <summary>
    /// Application bootstrapper for the .NET MAUI app.
    /// </summary>
    /// <remarks>
    /// Centralizes dependency registration, database wiring (EF Core + SQLCipher),
    /// and Prism initialization. Keep this class focused on composition and avoid
    /// adding application logic here.
    /// </remarks>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures the <see cref="MauiApp"/> instance used by all platforms.
        /// </summary>
        /// <returns>A fully configured MAUI application.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Initialize SQLCipher-native provider (via bundle_e_sqlcipher).
            // This registers the native SQLite provider so EF Core uses SQLCipher at runtime.
            Batteries_V2.Init();

            // Resolve a stable, app-private path for the encrypted SQLite database file.
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

            // Ensure migrations are discovered from the AppDbContext assembly when applying.
            var migrationsAssembly = typeof(AppDbContext).Assembly.FullName!;

            // Security: Provide the encryption key and an EF Core interceptor that applies it
            // whenever a connection is opened.
            builder.Services.AddSingleton<EncryptionKeyProvider>();
            builder.Services.AddSingleton<IEncryptionKeyProvider>(sp => sp.GetRequiredService<EncryptionKeyProvider>());
            builder.Services.AddSingleton<IEncryptionKeyWarmup>(sp => sp.GetRequiredService<EncryptionKeyProvider>());
            builder.Services.AddSingleton<SqliteEncryptionInterceptor>();

            // Use IServiceProvider overload so we can attach the interceptor.
            builder.Services.AddDbContextFactory<Data.AppDbContext>((sp, opts) =>
            {
                // Use a private cache to avoid leaking the page cache/key across connections
                // which can make a no-key connection appear to work when testing.
                opts.UseSqlite(
                    $"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Private",
                    b => b.MigrationsAssembly(migrationsAssembly));

                // Attach the interceptor that applies PRAGMA key on open.
                opts.AddInterceptors(sp.GetRequiredService<SqliteEncryptionInterceptor>());

                // Prefer a compiled EF model if present (generated via `dotnet ef dbcontext optimize`).
                // Avoids reflection and improves trimming safety.
                var compiledModelType = Type.GetType("XAMLDebuggingTechniques.Data.CompiledModels.AppDbContextModel, XAMLDebuggingTechniques");
                var instanceProp = compiledModelType?.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (instanceProp?.GetValue(null) is Microsoft.EntityFrameworkCore.Metadata.IModel compiledModel)
                {
                    opts.UseModel(compiledModel);
                }
            });

            // Optional convenience: resolve a scoped DbContext directly where DI scoping exists
            // (e.g., inside Prism view models with a scoped lifetime).
            builder.Services.AddScoped(sp =>
                sp.GetRequiredService<IDbContextFactory<Data.AppDbContext>>().CreateDbContext());

            builder
                .UseMauiApp<App>()
                .UsePrism(prism =>
                {
                    // On app start: ensure database exists, apply migrations, seed, etc.
                    prism.OnInitialized(async (container) =>
                    {
                        await DbInit.InitializeAsync(container);
                        // Warm up encryption key (avoid sync-over-async later)
                        var keyWarmup = Microsoft.Maui.Controls.Application.Current.GetService<IEncryptionKeyWarmup>();
                        if (keyWarmup is not null)
                            await keyWarmup.WarmUpAsync();

                        // Kick the lifetime orchestrator after infrastructure is ready.
                        var orchestrator = container.Resolve<IAppOrchestrator>();
                        await orchestrator.OnStartAsync();
                    });

                    prism
                        .RegisterTypes(container =>
                        {
                            // Lifetime orchestrator
                            container.RegisterSingleton<IAppOrchestrator, AppOrchestrator>();
                            // Infrastructure/services
                            container.RegisterSingleton<IDbPathProvider, DbPathProvider>();
                            container.RegisterSingleton<IKeyRotationService, KeyRotationService>();

                            // Navigation map: Page -> ViewModel
                            container.RegisterForNavigation<NavigationPage>();
                            container.RegisterForNavigation<MainPage, MainPageViewModel>();
                            container.RegisterForNavigation<SampleLauncherPage, SampleLauncherPageViewModel>();
                            container.RegisterForNavigation<BindingBasicsPage, BindingBasicsPageViewModel>();
                            container.RegisterForNavigation<BindingContextLostPage, BindingContextLostPageViewModel>();
                            container.RegisterForNavigation<MissingConverterPage, MissingConverterPageViewModel>();
                            container.RegisterForNavigation<NavigationFailPage, NavigationFailPageViewModel>();
                            container.RegisterForNavigation<VisualStateStallPage, VisualStateStallPageViewModel>();
                            container.RegisterForNavigation<TemplateBindingMysteryPage, TemplateBindingMysteryPageViewModel>();
                            container.RegisterForNavigation<ControlSwapLabPage, ControlSwapLabPageViewModel>();
                            container.RegisterForNavigation<DebugOverlayDemoPage, DebugOverlayDemoPageViewModel>();
                        })
                        // Initial navigation stack (rooted at a NavigationPage)
                        .CreateWindow("NavigationPage/SampleLauncherPage");
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            // Pipe logs to Debug output during development.
           // builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
