using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Prism.Navigation;

namespace ApplicationLifetime
{
    public interface IAppOrchestrator
    {
        // Lifetime
        Task OnStartAsync(CancellationToken ct = default);
        Task OnResumeAsync(CancellationToken ct = default);
        Task OnSleepAsync(CancellationToken ct = default);
        Task OnExitAsync(bool dueToCrash, CancellationToken ct = default);

        // Hardening
        void ReportUnhandledException(Exception ex, string channel, string? context = null);
        void ReportUnobservedTaskException(AggregateException ex, string channel, string? context = null);
        void ReportBindingFailure(string? property, Uri? source, int? line, string message);
    }

    [DebuggerDisplay("AppOrchestrator")]
    public sealed class AppOrchestrator : IAppOrchestrator
    {
        private readonly ILogger<AppOrchestrator> _log;
        private readonly INavigationService _nav;

        public AppOrchestrator(ILogger<AppOrchestrator> log, INavigationService nav)
        {
            _log = log;
            _nav = nav;
        }

        public Task OnStartAsync(CancellationToken ct = default)
        {
            // Place health checks, migrations gates, warmups.
            _log.LogInformation("App start sequence");
            return Task.CompletedTask;
        }

        public Task OnResumeAsync(CancellationToken ct = default)
        {
            _log.LogInformation("App resume");
            return Task.CompletedTask;
        }

        public Task OnSleepAsync(CancellationToken ct = default)
        {
            _log.LogInformation("App sleep");
            return Task.CompletedTask;
        }

        public Task OnExitAsync(bool dueToCrash, CancellationToken ct = default)
        {
            _log.LogWarning("App exit. Crash={Crash}", dueToCrash);
            return Task.CompletedTask;
        }

        public void ReportUnhandledException(Exception ex, string channel, string? context = null)
        {
            _log.LogCritical(ex, "Unhandled exception from {Channel}. Context={Context}", channel, context);
        }

        public void ReportUnobservedTaskException(AggregateException ex, string channel, string? context = null)
        {
            _log.LogError(ex, "Unobserved task exception from {Channel}. Context={Context}", channel, context);
        }

        public void ReportBindingFailure(string? property, Uri? source, int? line, string message)
        {
            _log.LogWarning("Binding failed {Property} at {Source} line {Line}: {Message}", property, source, line, message);
        }
    }
}
