# The origin of StoryBoarder

.NET MAUI already has the raw ability to move, fade, scale, rotate, and shape native-backed visual properties. What was missing was a light authoring loop for developers who know exactly which XAML values they want to animate.

StoryBoarder began with one constraint: **do not rebuild Blend**.

The better model was a compact editing deck—closer to a disciplined linear-editing surface than a full visual designer. The developer chooses the target and the properties. The deck records deliberate poses at claimed keyframes, keeps time visible, and handles the bookkeeping needed to turn those poses into coordinated motion.

## Shaping principles

- **The XAML stays the source of intent.** StoryBoarder works beside the view the developer is already shaping.
- **Keyframes are deliberate.** The timeline is for meaningful poses, not an invitation to micromanage every millisecond.
- **The controls stay compact.** A small quiver of property rigs should cover the common motion reefs without becoming a second IDE.
- **Playback proves the design.** Motion must be judged running in the app, where timing and composition meet.
- **Growth must preserve the inside line.** More properties and richer rigs can arrive later without burying the direct first-use workflow.

The visual language follows the same idea. The GlowcilliScope deck combines ordered structure, measured light, and dense instrument materials: a working surface with presence, not decoration pretending to be function.

**Measured motion. Light through structure.**

## Provenance

StoryBoarder was conceived by CodeCrafty and developed in collaboration with BreakerBytes (OpenAI Codex), 2026.

The project draws inspiration from the clarity of WPF, Silverlight, and Blend storyboards, as well as compact hardware and software editing surfaces. StoryBoarder is its own implementation and product direction.

Distribution uses the custom
[StoryBoarder Free Use License 1.0](../LICENSE.md).
