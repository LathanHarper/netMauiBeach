using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.AppModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.IO;
using System.Text;
using ApplicationLifetime;


namespace XAMLDebuggingTechniques.ViewModels
{
    [DebuggerDisplay("{DebugShort,nq}")]
    public class MainPageViewModel : BindableBase, INavigationAware, IInitializeAsync, IApplicationLifecycleAware
    {
        public AsyncDelegateCommand SeedCommand { get; }
        public AsyncDelegateCommand ReadCommand { get; }
        public AsyncDelegateCommand VerifyEncryptionCommand { get; }
        public AsyncDelegateCommand WipeAndRecreateEncryptedCommand { get; }

        private readonly IDbContextFactory<Data.AppDbContext> _dbFactory;
        private readonly IAppOrchestrator _orchestrator;

        public MainPageViewModel(IDbContextFactory<Data.AppDbContext> dbFactory, IAppOrchestrator orchestrator)
        {
            _dbFactory = dbFactory;
            _orchestrator = orchestrator;


            SeedCommand = new AsyncDelegateCommand(SeedAsync);
            ReadCommand = new AsyncDelegateCommand(ReadAsync);
            VerifyEncryptionCommand = new AsyncDelegateCommand(VerifyEncryptionAsync);
            WipeAndRecreateEncryptedCommand = new AsyncDelegateCommand(WipeAndRecreateEncryptedAsync);
        }

        // Debugger summary (avoid heavy allocations, no secrets)
        private string DebugShort => "MainPageViewModel [Commands=4]";

        public async Task<IEnumerable<Data.TodoItem>> LoadAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            return await db.TodoItems
                .OrderBy(x => x.Done)
                .ThenBy(x => x.Id)
                .ToListAsync();
        }
        private async Task SeedAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            db.TodoItems.Add(new Data.TodoItem { Title = "New set", Done = false });
            await db.SaveChangesAsync();
        }
        private async Task ReadAsync()
        {
            var records = await LoadAsync();
            // bind/use as needed
        }

        // Run this in Debug to verify SQLCipher is active and the file is encrypted
        private async Task VerifyEncryptionAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            // IMPORTANT: Open via EF so the interceptor applies PRAGMA key before any SQL
            await db.Database.OpenConnectionAsync();
            var conn = db.Database.GetDbConnection();

            var dbPath = conn.DataSource;
            Debug.WriteLine($"[Enc] DataSource = {dbPath}");

            // 0) Confirm runtime provider is SQLCipher (process-wide)
            try
            {
                using var mem = new SqliteConnection("Data Source=:memory:");
                await mem.OpenAsync();
                using var cv = mem.CreateCommand();
                cv.CommandText = "PRAGMA cipher_version;";
                var v = (await cv.ExecuteScalarAsync())?.ToString();
                Debug.WriteLine($"[Enc] (process) cipher_version => {v ?? "<null>"}");

                using var co = mem.CreateCommand();
                co.CommandText = "PRAGMA compile_options;";
                using var rdr = await co.ExecuteReaderAsync();
                var hasCodec = false;
                while (await rdr.ReadAsync())
                {
                    var opt = rdr.GetString(0);
                    if (opt.IndexOf("CODEC", StringComparison.OrdinalIgnoreCase) >= 0)
                        hasCodec = true;
                }
                Debug.WriteLine($"[Enc] (process) compile_options has SQLITE_HAS_CODEC: {hasCodec}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Enc] Provider check failed: {ex.GetType().Name}: {ex.Message}");
            }

            // 1) Positive checks on keyed connection
            try
            {
                using var vcmd = conn.CreateCommand();
                vcmd.CommandText = "PRAGMA cipher_version;";
                var cipherVersion = (await vcmd.ExecuteScalarAsync())?.ToString();
                Debug.WriteLine($"[Enc] (db) cipher_version => {cipherVersion ?? "<null>"}");

                using var icmd = conn.CreateCommand();
                icmd.CommandText = "PRAGMA cipher_integrity_check;";
                var integrity = (await icmd.ExecuteScalarAsync())?.ToString();
                Debug.WriteLine($"[Enc] cipher_integrity_check => {integrity ?? "<null>"}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Enc] Cipher PRAGMAs failed: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                await db.Database.CloseConnectionAsync();
            }

            // 2) Negative check: try to open WITHOUT a key
            try
            {
                // Use Private cache and disable pooling to avoid piggybacking on an already-keyed shared cache.
                using var noKey = new SqliteConnection($"Data Source={dbPath};Mode=ReadOnly;Cache=Private;Pooling=False");
                await noKey.OpenAsync();
                using var cmd = noKey.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master;";
                var count = await cmd.ExecuteScalarAsync();
                Debug.WriteLine($"[Enc] UNEXPECTED: Opened without key. sqlite_master count={count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Enc] As expected, opening without key failed: {ex.GetType().Name}: {ex.Message}");
            }

            // 3) Header check
            try
            {
                if (!File.Exists(dbPath))
                {
                    Debug.WriteLine($"[Enc] Header check skipped: file not found at {dbPath}");
                    return;
                }

                var len = new FileInfo(dbPath).Length;
                using var fs = File.OpenRead(dbPath);
                var header = new byte[Math.Min(16, (int)Math.Min(len, 16))];
                _ = await fs.ReadAsync(header, 0, header.Length);
                var headerText = Encoding.ASCII.GetString(header);
                var looksPlain = headerText.StartsWith("SQLite format 3", StringComparison.Ordinal);
                Debug.WriteLine($"[Enc] File length={len}, Header starts with 'SQLite format 3': {looksPlain} (false => encrypted)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Enc] Header check failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        // DEV-ONLY: delete the plaintext DB and let EF recreate it under a keyed connection.
        private async Task WipeAndRecreateEncryptedAsync()
        {
            await using var db = await _dbFactory.CreateDbContextAsync();
            var conn = db.Database.GetDbConnection();
            var dbPath = conn.DataSource;

            try
            {
                if (File.Exists(dbPath))
                {
                    Debug.WriteLine($"[Enc] Deleting {dbPath} …");
                    File.Delete(dbPath);
                }
                var wal = dbPath + "-wal";
                var shm = dbPath + "-shm";
                if (File.Exists(wal)) File.Delete(wal);
                if (File.Exists(shm)) File.Delete(shm);

                // Force EF to recreate using the already-registered encryption interceptor.
                var created = await db.Database.EnsureCreatedAsync();
                Debug.WriteLine($"[Enc] EnsureCreated: recreated={created}");

                // Sanity re-check
                await VerifyEncryptionAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Enc] Wipe/Recreate failed: {ex.GetType().Name}: {ex.Message}");
            }
        }

        async Task IInitializeAsync.InitializeAsync(INavigationParameters parameters)
        {



        }
        public async void OnNavigatedTo(INavigationParameters parameters)
        {
            // Ensure schema exists before any queries


        }
        public void OnNavigatedFrom(INavigationParameters parameters) { }


        // Prism-blessed lifecycle hooks pushed into the orchestrator (DI-based, no static locator).
        public async void OnResume()
        {
            try { await _orchestrator.OnResumeAsync(); } catch (Exception ex) { _orchestrator.ReportUnhandledException(ex, "VM.OnResume", nameof(MainPageViewModel)); }
        }

        public async void OnSleep()
        {
            try { await _orchestrator.OnSleepAsync(); } catch (Exception ex) { _orchestrator.ReportUnhandledException(ex, "VM.OnSleep", nameof(MainPageViewModel)); }
        }


    }
}
