# StoryBoarder

> A compact animation capture deck for .NET MAUI XAML.
>
> **Measured motion. Light through structure.**

![StoryBoarder Tow-In running as a compact Visual Studio motion-capture deck](assets/storyboarder-hero.png)

StoryBoarder helps a developer shape motion against the XAML they already own. Pick an element and property, pose it at deliberate moments, claim those moments as keyframes, then carry the captured motion into the app's playback experience.

The goal is not to rebuild Blend or introduce another visual-design environment. StoryBoarder stays close to the code and keeps the editing surface small: the developer chooses what matters, while the deck handles the pencil work between poses.

## The deck today

The private preview already carries reusable Pose and Paint + Depth rigs, a four-second claimed-key timeline, and the GlowcilliScope instrument surface. This is the current Tow-In window captured from the Visual Studio experimental instance—not a UI mockup.

<img src="assets/storyboarder-tow-in-current.png" alt="Current StoryBoarder Tow-In tool window" width="320">

## Current status

StoryBoarder Tow-In is in a **private preview/pilot**. The extension and its
setup material are shared directly with a small group of invited testers while
the authoring workflow and packaging are validated.

The companion
[`StoryBoarder.Motion` 0.4.0-preview.2 runtime](https://www.nuget.org/packages/StoryBoarder.Motion/0.4.0-preview.2)
is publicly available from NuGet.org so captured playback has a paved
distribution path. There is no public Tow-In VSIX installer yet.

## The ride

The preview workflow is intentionally direct:

1. Open the .NET MAUI XAML view you want to animate.
2. Choose a named element and one of its animatable properties.
3. Move through the compact timeline and pose the property.
4. Claim the deliberate moments as keyframes.
5. Save the playback definition and verify the motion in the app.

See [Getting started](docs/GETTING_STARTED.md) for the public preview boundary and [Origin](docs/ORIGIN.md) for the idea behind the tool.

## Provenance

StoryBoarder was conceived by CodeCrafty and developed in collaboration with BreakerBytes (OpenAI Codex), 2026.

StoryBoarder is distributed under the custom
[StoryBoarder Free Use License 1.0](LICENSE.md): use the unmodified tool freely,
ship applications with its unmodified runtime, and redistribute exact copies
while preserving its origin. Altered, rebranded, and substitute StoryBoarder
implementations are not permitted.
