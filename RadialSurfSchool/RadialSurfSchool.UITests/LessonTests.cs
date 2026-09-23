using RadialSurfSchool.UITests.Appium;
using Xunit;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace RadialSurfSchool.UITests;

public sealed class LessonTests(ITestOutputHelper output)
{
    [Fact]
    public Task AppOpensSmoke() => TestRun.RunAsync(nameof(AppOpensSmoke), output, async screen =>
        Assert.True(await screen.IsOpenAsync()));

    [Fact]
    public Task EnergySlider_UpdatesGauge() => TestRun.RunAsync(nameof(EnergySlider_UpdatesGauge), output, async screen =>
    {
        Assert.True(await screen.IsOpenAsync());
        await screen.SetEnergyEndpointAsync(maximum: false);
        Assert.Equal("0%", await screen.EnergyAsync("0%"));
        Assert.Equal("0%", await screen.GaugeValueAsync("0%"));
        await screen.SetEnergyEndpointAsync(maximum: true);
        Assert.Equal("100%", await screen.EnergyAsync("100%"));
        Assert.Equal("100%", await screen.GaugeValueAsync("100%"));
    });

    [Fact]
    public Task AngleSliders_UpdateReadouts() => TestRun.RunAsync(nameof(AngleSliders_UpdateReadouts), output, async screen =>
    {
        Assert.True(await screen.IsOpenAsync());
        await screen.SetStartEndpointAsync(maximum: true);
        Assert.Equal("180°", await screen.StartAsync("180°"));
        await screen.SetStartEndpointAsync(maximum: false);
        Assert.Equal("-180°", await screen.StartAsync("-180°"));
        await screen.SetSweepEndpointAsync(maximum: true);
        Assert.Equal("360°", await screen.SweepAsync("360°"));
        await screen.SetSweepEndpointAsync(maximum: false);
        Assert.Equal("-360°", await screen.SweepAsync("-360°"));
    });

    [Fact]
    public Task Reset_RestoresDefaults() => TestRun.RunAsync(nameof(Reset_RestoresDefaults), output, async screen =>
    {
        Assert.True(await screen.IsOpenAsync());
        await screen.SetEnergyEndpointAsync(maximum: false);
        await screen.SetStartEndpointAsync(maximum: true);
        await screen.SetSweepEndpointAsync(maximum: false);
        await screen.ResetAsync();
        Assert.Equal("60%", await screen.EnergyAsync("60%"));
        Assert.Equal("60%", await screen.GaugeValueAsync("60%"));
        Assert.Equal("-120°", await screen.StartAsync("-120°"));
        Assert.Equal("240°", await screen.SweepAsync("240°"));
        Assert.True(await screen.RideEnabledAsync());
        Assert.True(await screen.EnergyEnabledAsync());
    });

    [Fact]
    public Task Ride_Reaches100AndEnablesControls() => TestRun.RunAsync(nameof(Ride_Reaches100AndEnablesControls), output, async screen =>
    {
        Assert.True(await screen.IsOpenAsync());
        await screen.ResetAsync();
        await screen.RideAsync();
        Assert.Equal("100%", await screen.EnergyAsync("100%"));
        Assert.Equal("100%", await screen.GaugeValueAsync("100%"));
        Assert.True(await screen.RideEnabledAsync());
        Assert.True(await screen.EnergyEnabledAsync());
    });
}
