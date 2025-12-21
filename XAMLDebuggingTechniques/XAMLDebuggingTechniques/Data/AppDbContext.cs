using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace XAMLDebuggingTechniques.Data
{



    [DebuggerDisplay("{DebugShort,nq}")]
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// EF Core database context, small but grippy—built to hold fast in choppy seas.
        /// </summary>
        /// <param name="options">Context options composed at startup (provider, interceptors, etc.).</param>
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(TodoItem))]
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        /// <summary>
        /// Simple sample entity set. Start here; expand the quiver as the lineup grows.
        /// </summary>
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        // Debug-only helpers for rich Watch/Locals views
        private string DebugShort
        {
            get
            {
                string provider = Database?.ProviderName ?? "<provider>";
                string source = SafeDataSource ?? "<datasource>";
                return $"AppDbContext [{provider}] @ {source}";
            }
        }

        private string? SafeDataSource
        {
            get
            {
                try { return Database?.GetDbConnection()?.DataSource; }
                catch { return null; }
            }
        }
    }
}
