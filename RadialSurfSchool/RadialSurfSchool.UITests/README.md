# Radial surf school: local Android UI tests

This separate .NET 10 project uses Appium/UiAutomator2 and xUnit. It is intentionally
outside the main app solution: build and run it by project path. The installed app
must use package `com.netmauibeach.radialsurfschool`.

## Prepare one witnessed environment

Use the existing Visual Studio-created emulator agreed with the operator. Record
the actual Visual Studio SDK and JDK settings, the full adb executable path, AVD
name, explicit live serial, and Appium/UiAutomator2 versions. A conventional SDK
directory or an old test receipt is insufficient. Start the server separately with
`ANDROID_HOME`, `ANDROID_SDK_ROOT`, `JAVA_HOME`, and `PATH` pointing to that exact
toolchain. Preserve app data.

Build and install the exact local app before running this suite. Keep its SHA-256
and install evidence alongside the test evidence. For fast deployment, record the
APK and deployed assemblies together as the payload. Debug proof is separate from
packaged Release linking/AOT proof.

Set these variables in the test shell after the emulator and server are ready:

```powershell
$env:APPIUM_SERVER_URI = 'http://127.0.0.1:4723/'
$env:APPIUM_ANDROID_UDID = '<verified live serial>'
$env:APPIUM_APP_PACKAGE = 'com.netmauibeach.radialsurfschool'
$env:APPIUM_APP_ACTIVITY = '<verified installed launch activity>'
# Optional: absolute directory for this run's private evidence.
$env:APPIUM_EVIDENCE_ROOT = '<absolute evidence directory>'
```

All four connection settings are required. Only localhost HTTP(S) is accepted.
The suite creates one session per test with `noReset=true` and `forceAppLaunch=true`.
It does not build/install the app, launch an emulator, start Appium, reset app data,
restart adb, or reconnect. A session-creation or driver-command failure prevents
additional session attempts in that test process; expected lookup/assertion
failures remain separate. Keep each run bounded with an exact filter.

## Compile and discover without a device

Run from this directory; these commands do not create an Appium session:

```powershell
dotnet build .\RadialSurfSchool.UITests.csproj
dotnet test .\RadialSurfSchool.UITests.csproj --no-build --list-tests
dotnet test .\RadialSurfSchool.UITests.csproj --no-build --list-tests --filter 'FullyQualifiedName=RadialSurfSchool.UITests.LessonTests.AppOpensSmoke'
```

Pinned packages: Appium.WebDriver 8.1.0, Selenium.WebDriver 4.36.0,
Microsoft.NET.Test.Sdk 18.8.1, xUnit 2.9.3, and its Visual Studio runner 3.1.5.
The Appium/Selenium pairing follows the installed client's package dependency;
see the [Appium client compatibility guidance](https://github.com/appium/dotnet-client).
Interaction commands use [UiAutomator2 native gestures](https://github.com/appium/appium-uiautomator2-driver/blob/master/docs/android-mobile-gestures.md).

## Run the smoke first

After the required environment and exact payload are verified:

```powershell
dotnet test .\RadialSurfSchool.UITests.csproj --no-build --filter 'FullyQualifiedName=RadialSurfSchool.UITests.LessonTests.AppOpensSmoke' --logger 'trx;LogFileName=app-opens.trx' --results-directory .\TestArtifacts\Results
```

Confirm the smoke succeeds before running a behavior test. Substitute exactly one
of these full names in that command's filter, using a distinct TRX filename:

| FullyQualifiedName | Assertion |
| --- | --- |
| `RadialSurfSchool.UITests.LessonTests.EnergySlider_UpdatesGauge` | Energy endpoints update both the readout and gauge value. |
| `RadialSurfSchool.UITests.LessonTests.AngleSliders_UpdateReadouts` | Start and sweep endpoints include negative angles. |
| `RadialSurfSchool.UITests.LessonTests.Reset_RestoresDefaults` | Changed controls return to 60%, −120°, and 240°. |
| `RadialSurfSchool.UITests.LessonTests.Ride_Reaches100AndEnablesControls` | The animation reaches 100% and restores both controls. |

The catalog tries accessibility ID, resource ID, and package-qualified resource ID.
Bounded native scrolling reaches controls below the fold. `GaugeValueLabel` is
scoped to `LessonScroll`, the observable root, because native platforms may flatten
ContentPage/ContentView wrappers. There is exactly one gauge in this lesson.
Review the Android XML before claiming those locator recipes are witnessed.
Readout assertions do not prove segment positions or colors; inspect screenshots
for visual proof.

Each test writes `session.json` beneath ignored `TestArtifacts` (or the explicit
evidence root). Failures preserve the first exception, screenshot, XML, compact
tree, and any capture/cleanup error without hiding the original failure. If no
session exists, screenshot and XML cannot be captured. Request TRX explicitly.

On emulator, adb, or Appium connectivity failure, stop the affected run, preserve
the first error, and ask the operator to repair the agreed local environment.
Do not retry until the agreed environment is repaired. Healthy-connectivity
assertion failures can be debugged normally. No test creates persistent records.
Compilation and discovery alone are not launch or behavior proof.
