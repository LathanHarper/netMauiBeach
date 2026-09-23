using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace RadialSurfSchool.UITests.Locators;

internal sealed record UiTarget(string Name, string AutomationId, UiTarget? Parent = null)
{
    public IEnumerable<By> Recipes(string package)
    {
        yield return MobileBy.AccessibilityId(AutomationId);
        yield return By.Id(AutomationId);
        yield return By.Id($"{package}:id/{AutomationId}");
    }

    // Both Android MAUI exposures are supported without depending on translated text.
    public IEnumerable<By> ScrollRecipes(string package)
    {
        var container = "new UiScrollable(new UiSelector().scrollable(true).instance(0)).setMaxSearchSwipes(5)";
        foreach (var selector in new[]
                 {
                     $"resourceId(\"{AutomationId}\")",
                     $"resourceId(\"{package}:id/{AutomationId}\")",
                     $"description(\"{AutomationId}\")"
                 })
            yield return MobileBy.AndroidUIAutomator($"{container}.scrollIntoView(new UiSelector().{selector})");
    }
}

internal static class LessonTargets
{
    public static readonly UiTarget Root = new("Lesson scroll container", "LessonScroll");
    // This lesson contains one gauge. ContentView may not survive native tree flattening;
    // keep the inner label scoped to the observable lesson root, not an absent wrapper.
    public static readonly UiTarget GaugeValue = new("Lesson > gauge value", "GaugeValueLabel", Root);
    public static readonly UiTarget Energy = new("Energy slider", "EnergySlider");
    public static readonly UiTarget EnergyLabel = new("Energy readout", "EnergyLabel");
    public static readonly UiTarget Start = new("Start angle slider", "StartAngleSlider");
    public static readonly UiTarget StartLabel = new("Start angle readout", "StartAngleLabel");
    public static readonly UiTarget Sweep = new("Sweep slider", "SweepAngleSlider");
    public static readonly UiTarget SweepLabel = new("Sweep readout", "SweepAngleLabel");
    public static readonly UiTarget Ride = new("Ride a wave", "RideWaveButton");
    public static readonly UiTarget Reset = new("Reset lesson", "ResetLessonButton");
}
