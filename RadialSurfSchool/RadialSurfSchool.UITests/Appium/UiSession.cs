using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using RadialSurfSchool.UITests.Locators;

namespace RadialSurfSchool.UITests.Appium;

internal sealed class UiSession(AndroidDriver driver, string package)
{
    public async Task<AppiumElement> WaitAsync(UiTarget target, Func<AppiumElement, bool>? condition = null)
    {
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < TimeSpan.FromSeconds(10))
        {
            try
            {
                var element = Find(target);
                if (element is not null && element.Displayed && (condition is null || condition(element)))
                    return element;
            }
            catch (StaleElementReferenceException) { }
            await Task.Delay(100);
        }
        throw new TimeoutException($"{target.Name} did not satisfy its visible-state condition within 10 seconds.");
    }

    public async Task<AppiumElement> RevealAsync(UiTarget target)
    {
        var existing = Find(target);
        if (existing is not null && existing.Displayed)
            return existing;

        // Scrolling uses the label's ID; final lookup still enforces its parent scope.
        foreach (var recipe in target.ScrollRecipes(package))
        {
            try
            {
                driver.FindElement(recipe);
                return await WaitAsync(target);
            }
            catch (NoSuchElementException) { }
        }
        throw new NoSuchElementException($"Cannot scroll to {target.Name} using its AutomationId.");
    }

    public async Task TapAsync(UiTarget target) => (await RevealAsync(target)).Click();

    public async Task SetSliderEndpointAsync(UiTarget target, bool maximum)
    {
        var slider = await RevealAsync(target);
        if (!slider.Enabled)
            throw new InvalidOperationException($"{target.Name} is disabled.");

        // Native SeekBar taps at either end exercise the user's slider interaction.
        driver.ExecuteScript("mobile: clickGesture", new Dictionary<string, object>
        {
            ["x"] = slider.Location.X + (maximum ? slider.Size.Width - 2 : 1),
            ["y"] = slider.Location.Y + slider.Size.Height / 2
        });
    }

    public async Task<string> TextAsync(UiTarget target, string? expected = null)
    {
        await RevealAsync(target);
        return (await WaitAsync(target, expected is null ? null : element => element.Text == expected)).Text;
    }

    public async Task<bool> EnabledAsync(UiTarget target)
    {
        await RevealAsync(target);
        return (await WaitAsync(target, element => element.Enabled)).Enabled;
    }

    private AppiumElement? Find(UiTarget target)
    {
        ISearchContext scope = driver;
        if (target.Parent is not null)
        {
            var parent = Find(target.Parent);
            if (parent is null) return null;
            scope = parent;
        }
        foreach (var recipe in target.Recipes(package))
        {
            var element = scope.FindElements(recipe).OfType<AppiumElement>().FirstOrDefault();
            if (element is not null) return element;
        }
        return null;
    }
}
