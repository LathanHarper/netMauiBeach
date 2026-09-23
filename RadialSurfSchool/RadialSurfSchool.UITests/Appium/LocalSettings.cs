namespace RadialSurfSchool.UITests.Appium;

internal sealed record LocalSettings(Uri Server, string Udid, string Package, string Activity)
{
    public const string ExpectedPackage = "com.netmauibeach.radialsurfschool";

    public static LocalSettings Read()
    {
        var serverText = Required("APPIUM_SERVER_URI");
        if (!Uri.TryCreate(serverText, UriKind.Absolute, out var server) ||
            !server.IsLoopback || server.Scheme is not ("http" or "https") ||
            server.UserInfo.Length != 0 || server.Query.Length != 0 || server.Fragment.Length != 0)
            throw new InvalidOperationException("APPIUM_SERVER_URI must be an explicit localhost HTTP(S) URI without credentials, query, or fragment.");

        var package = Required("APPIUM_APP_PACKAGE");
        if (package != ExpectedPackage)
            throw new InvalidOperationException($"APPIUM_APP_PACKAGE must be {ExpectedPackage}.");

        return new(server, Required("APPIUM_ANDROID_UDID"), package, Required("APPIUM_APP_ACTIVITY"));
    }

    private static string Required(string name) =>
        Environment.GetEnvironmentVariable(name) is { } value && !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new InvalidOperationException($"Set {name} explicitly before running a device test.");
}
