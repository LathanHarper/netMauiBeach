# Getting started with StoryBoarder

StoryBoarder Tow-In is currently a private preview. This page describes the
shape of the authoring experience while publishing the runtime dependency used
by generated playback.

## Before you paddle out

Private-preview participants receive the tested VSIX, its release-specific
setup guide, and supported-version notes directly from the project.

The playback runtime is public on NuGet.org:

```powershell
dotnet add YourApp.csproj package StoryBoarder.Motion `
  --version 0.4.0-preview.2
```

For the authoring workflow, have a .NET MAUI solution with a XAML view you can edit and run. Choose a small, visible motion first—a label translating between two poses is the cleanest way to learn the deck.

## Preview workflow

1. Open the target XAML view in Visual Studio.
2. Identify the element and property you intend to animate.
3. Open the StoryBoarder Tow-In deck and select that target.
4. Set the first pose and claim its keyframe.
5. Move to another time, set the next pose, and claim that keyframe.
6. Save the playback definition using the preview guide.
7. Run the app and inspect the complete motion—not only the endpoints.

Start with one element and one property. Once its timing reads clearly, add another property or element and judge the coordinated result in the running app.

## What stays private today

The Tow-In VSIX, implementation source, tester scripts, and internal release
process are not distributed through this public repository. The
`StoryBoarder.Motion` runtime package is distributed through NuGet.org.

When the Tow-In preview widens, this page will contain the authoritative Visual
Studio prerequisites, extension installation steps, first-run walkthrough, and
update path.

## License

StoryBoarder uses the custom
[StoryBoarder Free Use License 1.0](../LICENSE.md). It permits free use,
commercial applications built with the unmodified runtime, and redistribution
of exact unmodified copies while preserving the license and provenance.
