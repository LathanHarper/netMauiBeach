# Getting started with StoryBoarder

StoryBoarder Tow-In `0.4.1` is a free public preview for Visual Studio 2026.
The extension captures motion in the IDE; the separate
`StoryBoarder.Motion` `0.4.0-preview.2` package runs the generated playback in
your .NET MAUI app.

## Before you paddle out

Have these ready:

- Visual Studio 2026 Stable or Insiders on amd64, version 18.0 or newer
- .NET 10 and the .NET MAUI 10 workload
- A .NET MAUI XAML view you can edit and run with Hot Reload
- A commit or backup of that view before StoryBoarder edits it

Install
[`StoryBoarder Tow-In` 0.4.1](https://marketplace.visualstudio.com/items?itemName=CodeCrafty.StoryBoarder)
from the Visual Studio Marketplace. You can also open
**Extensions → Manage Extensions**, search for **StoryBoarder Tow-In**, install
it, and restart Visual Studio when prompted.

Add the public playback runtime to the MAUI app:

```powershell
dotnet add YourApp.csproj package StoryBoarder.Motion `
  --version 0.4.0-preview.2
```

If the project explicitly pins its MAUI version, use Microsoft.Maui.Controls
`10.0.90` or newer.

## First launch: the courtesy gate

The first time StoryBoarder opens for a Visual Studio profile, it asks
**What's da Password?** Enter:

```text
1234
```

This is an invitational courtesy gate, not security or access control. The code
is intentionally published here and can be discovered inside the VSIX.
StoryBoarder records only that the current Visual Studio profile accepted the
gate; it does not store or transmit the password. Stable, Insiders, and
Experimental profiles keep separate settings, so each profile can ask once.

## Preview workflow

1. Open the target XAML view in Visual Studio.
2. Identify the element and property you intend to animate.
3. Open **View → Other Windows → StoryBoarder Tow-In** and select that target.
4. Set the first pose and claim its keyframe.
5. Move to another time, set the next pose, and claim that keyframe.
6. Save the playback definition.
7. Run the app and inspect the complete motion—not only the endpoints.

StoryBoarder actions such as capture, shaping, key navigation, saving, and
clearing can edit and save the active XAML document so Hot Reload can show the
result. The extension does not mutate documents in the background.

Start with one element and one property. A label translating between two poses
is the cleanest first ride. Once its timing reads clearly, add another property
or element and judge the coordinated result in the running app.

## Public preview boundary

The StoryBoarder Tow-In `0.4.1` VSIX is distributed through the Visual Studio
Marketplace. The `StoryBoarder.Motion` `0.4.0-preview.2` runtime is distributed
through NuGet.org.

This repository remains the public home for documentation, media, licensing,
and release guidance. The authoring extension's implementation source, internal
tester scripts, and release process remain private. Public preview behavior and
supported authoring surfaces can change as feedback shapes later versions;
Visual Studio's extension manager supplies Marketplace updates.

## License

StoryBoarder uses the custom
[StoryBoarder Free Use License 1.0](../LICENSE.md). It permits free use,
commercial applications built with the unmodified runtime, and redistribution
of exact unmodified copies while preserving the license and provenance.
