using RadialSurfSchool.UITests.Appium;
using RadialSurfSchool.UITests.Locators;

namespace RadialSurfSchool.UITests.Screens;

internal sealed class LessonScreen(UiSession ui)
{
    public async Task<bool> IsOpenAsync() => (await ui.WaitAsync(LessonTargets.Root)).Displayed;
    public Task ResetAsync() => ui.TapAsync(LessonTargets.Reset);
    public Task RideAsync() => ui.TapAsync(LessonTargets.Ride);
    public Task SetEnergyEndpointAsync(bool maximum) => ui.SetSliderEndpointAsync(LessonTargets.Energy, maximum);
    public Task SetStartEndpointAsync(bool maximum) => ui.SetSliderEndpointAsync(LessonTargets.Start, maximum);
    public Task SetSweepEndpointAsync(bool maximum) => ui.SetSliderEndpointAsync(LessonTargets.Sweep, maximum);
    public Task<string> EnergyAsync(string expected) => ui.TextAsync(LessonTargets.EnergyLabel, expected);
    public Task<string> GaugeValueAsync(string expected) => ui.TextAsync(LessonTargets.GaugeValue, expected);
    public Task<string> StartAsync(string expected) => ui.TextAsync(LessonTargets.StartLabel, expected);
    public Task<string> SweepAsync(string expected) => ui.TextAsync(LessonTargets.SweepLabel, expected);
    public Task<bool> RideEnabledAsync() => ui.EnabledAsync(LessonTargets.Ride);
    public Task<bool> EnergyEnabledAsync() => ui.EnabledAsync(LessonTargets.Energy);
}
