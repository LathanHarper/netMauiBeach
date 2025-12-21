using Android.App;
using Android.Content.PM;
using Android.OS;

namespace XAMLDebuggingTechniques
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        // Intentionally blank: MAUI wires the activity to the cross-platform UI.
        // Use platform-specific services or permissions here when required.
    }
}
