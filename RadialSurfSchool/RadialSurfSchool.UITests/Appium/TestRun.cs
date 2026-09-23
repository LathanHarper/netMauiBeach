using System.Text.Json;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using RadialSurfSchool.UITests.Screens;
using Xunit.Abstractions;

namespace RadialSurfSchool.UITests.Appium;

internal static class TestRun
{
    // A failed session/driver command stops this process from opening another session.
    private static Exception? _sessionFailure;

    public static async Task RunAsync(string test, ITestOutputHelper output, Func<LessonScreen, Task> action)
    {
        if (_sessionFailure is not null)
            throw new InvalidOperationException("A previous Appium session failed. Stop and repair the agreed environment before a new run.", _sessionFailure);

        var root = Environment.GetEnvironmentVariable("APPIUM_EVIDENCE_ROOT")
                   ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../TestArtifacts"));
        var folder = Path.Combine(root, $"{DateTime.UtcNow:yyyyMMdd-HHmmssfff}-{test}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        output.WriteLine($"Evidence: {folder}");
        var started = DateTimeOffset.UtcNow;
        LocalSettings? settings = null;
        AndroidDriver? driver = null;
        string? sessionId = null;
        Exception? failure = null;
        try
        {
            settings = LocalSettings.Read();
            var options = new AppiumOptions { PlatformName = "Android", AutomationName = "UiAutomator2" };
            options.AddAdditionalAppiumOption("udid", settings.Udid);
            options.AddAdditionalAppiumOption("appPackage", settings.Package);
            options.AddAdditionalAppiumOption("appActivity", settings.Activity);
            options.AddAdditionalAppiumOption("noReset", true);
            options.AddAdditionalAppiumOption("forceAppLaunch", true);
            options.AddAdditionalAppiumOption("newCommandTimeout", 120);
            options.AddAdditionalAppiumOption("adbExecTimeout", 30000);
            try
            {
                driver = new AndroidDriver(settings.Server, options, TimeSpan.FromSeconds(60));
            }
            catch (Exception exception)
            {
                _sessionFailure = exception;
                throw;
            }
            sessionId = driver.SessionId.ToString();
            await action(new LessonScreen(new UiSession(driver, settings.Package)));
        }
        catch (Exception exception)
        {
            failure = exception;
            if (exception is WebDriverException and not (NoSuchElementException or StaleElementReferenceException))
                _sessionFailure = exception;
            await File.WriteAllTextAsync(Path.Combine(folder, "failure.txt"), exception.ToString());
            if (driver is not null)
            {
                Capture(() => driver.GetScreenshot().SaveAsFile(Path.Combine(folder, "failure.png")), "screenshot");
                Capture(() =>
                {
                    var xml = driver.PageSource;
                    File.WriteAllText(Path.Combine(folder, "failure.xml"), xml);
                    var lines = XDocument.Parse(xml).Descendants().Select(node =>
                        $"{node.Name.LocalName} " + string.Join(" ", node.Attributes().Where(attribute =>
                            new[] { "resource-id", "content-desc", "text", "displayed", "enabled", "bounds" }.Contains(attribute.Name.LocalName))));
                    File.WriteAllLines(Path.Combine(folder, "failure.tree.txt"), lines);
                }, "page-source");
            }
            throw;
        }
        finally
        {
            Exception? cleanupFailure = null;
            try { driver?.Quit(); }
            catch (Exception exception)
            {
                cleanupFailure = exception;
                _sessionFailure = exception;
                await File.WriteAllTextAsync(Path.Combine(folder, "cleanup-error.txt"), exception.ToString());
            }
            var metadata = new
            {
                Test = test, ExactFilter = $"FullyQualifiedName=RadialSurfSchool.UITests.LessonTests.{test}",
                Started = started, Finished = DateTimeOffset.UtcNow,
                Settings = settings, SessionId = sessionId,
                NoReset = true, ForceAppLaunch = true,
                AppiumClient = typeof(AndroidDriver).Assembly.GetName().Version?.ToString(),
                DotNetRuntime = Environment.Version.ToString(),
                Outcome = failure is null && cleanupFailure is null ? "Passed" : "Failed"
            };
            await File.WriteAllTextAsync(Path.Combine(folder, "session.json"),
                JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));
            if (failure is null && cleanupFailure is not null)
                throw new InvalidOperationException("Appium session cleanup failed; see cleanup-error.txt.", cleanupFailure);
        }

        void Capture(Action capture, string name)
        {
            try { capture(); }
            catch (Exception exception)
            {
                File.WriteAllText(Path.Combine(folder, $"{name}-error.txt"), exception.ToString());
                output.WriteLine($"Could not capture {name}: {exception.Message}");
            }
        }
    }
}
