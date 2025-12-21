using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace XAMLDebuggingTechniques.ViewModels
{
    public class NavigationFailPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;

        private string _title = "Navigation Fail – route check";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _targetRoute = "BindingBasicsPage";
        public string TargetRoute
        {
            get => _targetRoute;
            set => SetProperty(ref _targetRoute, value);
        }

        private string _lastNavigationResult = "Idle";
        public string LastNavigationResult
        {
            get => _lastNavigationResult;
            set => SetProperty(ref _lastNavigationResult, value);
        }

        public ObservableCollection<RouteStatus> KnownRoutes { get; } = new();
        public ObservableCollection<string> NavigationStack { get; } = new();
        public ObservableCollection<NavigationEventRow> NavigationEvents { get; } = new();

        public DelegateCommand NavigateCommand { get; }
        public DelegateCommand PopStackCommand { get; }

        private int _attemptCounter;

        public NavigationFailPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));

            // Use async lambda to avoid missing API for FromAsyncHandler in this Prism version
            NavigateCommand = new DelegateCommand(async () => await ExecuteNavigationAsync());
            PopStackCommand = new DelegateCommand(PopStack, () => NavigationStack.Any());
            NavigationStack.CollectionChanged += (_, __) => PopStackCommand.RaiseCanExecuteChanged();

            KnownRoutes.Add(new RouteStatus("BindingBasicsPage", true, "Property path demos"));
            KnownRoutes.Add(new RouteStatus("BindingContextLostPage", true, "Context leash"));
            KnownRoutes.Add(new RouteStatus("MissingConverterPage", true, "Converters"));
            KnownRoutes.Add(new RouteStatus("NavigationFailPage", true, "You are here"));
            KnownRoutes.Add(new RouteStatus("VisualStateStallPage", false, "Intentionally unregistered"));
            KnownRoutes.Add(new RouteStatus("DebugOverlayDemoPage", true, "Overlay HUD"));
        }

        private async Task ExecuteNavigationAsync()
        {
            _attemptCounter++;
            var trimmed = TargetRoute?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                LastNavigationResult = "No route entered";
                return;
            }

            try
            {
                // Use Prism's navigation service and inspect the returned result
                var navResult = await _navigationService.NavigateAsync(trimmed);

                if (navResult is null)
                {
                    LastNavigationResult = $"Null navigation result for '{trimmed}'";
                    NavigationEvents.Add(new NavigationEventRow(_attemptCounter, trimmed, "NullResult"));
                    return;
                }

                if (navResult.Success)
                {
                    NavigationStack.Add(trimmed + " (navigated)");
                    LastNavigationResult = $"Success: {trimmed}";
                    NavigationEvents.Add(new NavigationEventRow(_attemptCounter, trimmed, "OnNavigatedTo"));
                }
                else
                {
                    // navResult.Exception may contain the underlying reason (route not registered, exception in ctor, etc.)
                    var error = navResult.Exception?.Message ?? "Unknown failure";
                    LastNavigationResult = $"Fail: {trimmed} -> {error}";
                    NavigationEvents.Add(new NavigationEventRow(_attemptCounter, trimmed, "Failure:" + error));
                }
            }
            catch (Exception ex)
            {
                // Defensive: some Prism hosts may throw; surface the exception message
                LastNavigationResult = $"Exception: {ex.GetType().Name}: {ex.Message}";
                NavigationEvents.Add(new NavigationEventRow(_attemptCounter, trimmed, "Exception:" + ex.Message));
            }
        }

        private void PopStack()
        {
            if (NavigationStack.Any())
            {
                var removed = NavigationStack[^1];
                NavigationStack.RemoveAt(NavigationStack.Count - 1);
                NavigationEvents.Add(new NavigationEventRow(_attemptCounter, removed, "PopAsync"));
            }
        }

        public void OnNavigatedTo(INavigationParameters parameters) { }
        public void OnNavigatedFrom(INavigationParameters parameters) { }
    }

    public record RouteStatus(string Route, bool IsRegistered, string Notes);
    public record NavigationEventRow(int Attempt, string Route, string EventName);
}
