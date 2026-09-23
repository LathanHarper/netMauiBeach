# Radial Surf School

A beginner .NET MAUI Beach sample: build a radial gauge by placing one trapezoid, moving its rotation anchor, and repeating it around an arc.

The visuals are shared XAML. A little shared C# supplies segment state, angle calculations, bindable properties, and animation. The sample uses native .NET 10 MAUI controls and Shapes, with no drawing surface or additional UI framework.

## Ride the sample

For the co-host preview, clone this repository (or pull the latest `main` if you already have it), then enter the sample directory:

```powershell
git clone https://github.com/LathanHarper/netMauiBeach.git
cd netMauiBeach/RadialSurfSchool
```

Open `RadialSurfSchool.sln` in Visual Studio with the .NET 10 SDK and the .NET MAUI workload installed. The draft pins SDK 10.0.401 and MAUI 10.0.20. Select your configured local target and run the app.

To compile the Windows app locally, run this from the `RadialSurfSchool` directory so its SDK pin applies:

```powershell
dotnet build RadialSurfSchool/RadialSurfSchool.csproj -f net10.0-windows10.0.19041.0
```

The main page provides three adjustments:

| Adjustment | What changes |
| --- | --- |
| Value | Lights the segments from 0 to 100 percent. |
| Start angle | Moves the beginning of the arc. Zero points to twelve o'clock. |
| Sweep | Changes the arc's extent and direction. Positive is clockwise; negative is counterclockwise. |

**Reset** returns to 60 percent, a -120 degree start, and a 240 degree sweep. **Ride a wave** animates from 0 to 100 percent once. Reset can cancel the animation.

The reusable `RadialGauge` exposes `Value` (default `0`, clamped to `0` through `100`), `StartAngle` (default `-120`), and `SweepAngle` (default `240`, clamped to `-360` through `360`). It deliberately keeps **13 segments** so the geometry and individual elements are easy to see. Lit segment count rounds to the nearest whole segment; the numeric readout represents the continuous value.

The first board is a fixed 260 by 260 layout. Its trapezoids are 28 by 32, centered at radius 98. In `Controls/RadialGauge.xaml`, `TranslationY="-98"` and `AnchorY="3.5625"` describe the same central pivot: `0.5 + 98 / 32`. A zero sweep stacks all the pieces; very small sweeps overlap them. Full circles use distinct positions at the seam.

```xml
<controls:RadialGauge Value="75" StartAngle="-90" SweepAngle="180" />
```

Declare `xmlns:controls="clr-namespace:RadialSurfSchool.Controls"` on the consuming page.

## What to learn

1. Define one trapezoid in XAML.
2. Put its rotation pivot outside its own bounds with `AnchorY`.
3. Rotate it around that pivot.
4. Repeat the same shape at evenly spaced angles.
5. Let shared state choose which segments are lit.
6. Wrap the result in a `ContentView` with three useful properties.

Follow the [lesson plan](docs/lesson-plan.md) for the teaching order and small experiments.

For a first code walkthrough, start with [the shape and pivot](RadialSurfSchool/Controls/RadialGauge.xaml), then [the angle and value state](RadialSurfSchool/Controls/RadialGauge.xaml.cs), and finally [the demo page](RadialSurfSchool/MainPage.xaml).

Each segment remains an individual MAUI element, so it can later gain its own transforms and behavior. That also means more elements as the segment count or number of gauges grows. This lesson establishes the technique; later samples can measure richer view-based designs and owner-drawn alternatives against the same workload.

## Readiness

This is a **public co-host preview**, shared September 23, 2026 for episode preparation. Windows Debug compilation and an interactive native Windows check have passed. **Coded Android Appium functionality verification is pending**, along with iOS and Mac validation. The finished episode and a fully validated release are still in preparation.

The separate `RadialSurfSchool.UITests` project owns Android UI checks and is intentionally outside the app solution. Its five tests compile and are discoverable; they have not yet run against the app on Android. See [the UI test README](RadialSurfSchool.UITests/README.md) for the explicit local device and payload requirements.

Background reading: Microsoft's [rotation and anchor documentation](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/animation/basic?view=net-maui-10.0) and [bindable layouts](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/layouts/bindablelayout?view=net-maui-10.0).

Stock template and font attributions are listed in [third-party notices](THIRD-PARTY-NOTICES.md).
