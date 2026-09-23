# Radial Surf School: from one segment to a shared control

## The promise

By the end, a learner can explain how an ordinary XAML shape becomes a radial gauge, adjust its arc without redrawing its geometry, and reuse the control on another page.

Use the finished gauge as a short opening reveal, then return to one shape. Keep the first lesson focused on placement, rotation, repetition, and state.

## 1. Shape the first board

Show the trapezoid in XAML. Its outline lives in the visual definition; the segment state does not calculate every corner of a rotated polygon.

Change its fill briefly so learners can identify the element on screen. Explain that the same placement technique can later hold a different silhouette.

## 2. Move the pivot

Show what rotating around the shape's center does. Then place the shape above the intended dial center and move its rotation anchor down to that center.

`AnchorX = 0.5` keeps the pivot on the shape's horizontal centerline. `AnchorY` is measured relative to the shape's height and may be outside its bounds. When radius means the distance from the dial center to the segment center:

```text
AnchorY = 0.5 + radius / segmentHeight
```

The layout position and the anchor must describe the same pivot. Mark that pivot visually during the explanation: the segment travels around it while retaining its own simple geometry.

## 3. Turn one segment

Change `Rotation` and watch the same shape travel around the dial. Establish the sample's convention: zero at twelve o'clock, positive angles clockwise.

Try a quarter turn and a negative angle. This is the essential technique students should understand before seeing a collection or a bindable property.

## 4. Repeat the stamp

Introduce 13 segment states and one shared XAML visual template. Every segment has the same basic placement and anchor; its angle identifies its place along the arc.

For an open arc with both endpoints included:

```text
angle = startAngle + index * sweepAngle / (count - 1)
```

The default `-120` start and `240` sweep create an arc centered around twelve o'clock. Changing the sign of the sweep reverses its direction. A zero sweep intentionally collapses the positions together.

For a complete `360` or `-360` sweep, use `count` as the divisor so the final segment does not duplicate the first at the seam. Keep this boundary rule inside the control.

## 5. Give the dial a value

Introduce `Value` in the range 0 to 100. Shared C# state selects the lit segments, and XAML renders their appearance. The center readout helps show that a continuous value is represented by a finite number of segments.

Move the value slider slowly. Use **Ride a wave** to demonstrate one value animation driving the same state. Keep per-segment choreography for a later lesson.

## 6. Hand over a reusable control

Wrap the visuals in `RadialGauge`, a shared `ContentView`. Show its three bindable properties: `Value`, `StartAngle`, and `SweepAngle`. The page's sliders consume those properties without knowing the trapezoid coordinates or anchor calculation.

Useful experiments:

- Set a `180` sweep to make a semicircle.
- Use a negative sweep to reverse the progression.
- Move the start angle while keeping the same value.
- Reset, then run the value animation again.

## Save the bigger waves

Keep the first app to one page, one gauge, one small segment-state class, and shared XAML visuals. Fixed segment count, dimensions, and palette keep the lesson readable.

Later lessons can explore denser scales, independent segment motion, palettes, glyphs, and owner drawing. Individual elements buy independent behavior and carry a view-count cost. Measure creation time, memory, and animation behavior before claiming one implementation wins.

## Delivery checkpoint

This public co-host preview is available for lesson preparation. Windows Debug compilation and an interactive native Windows check have passed. Coded Android functionality verification is pending. Before presenting Android behavior as verified, exercise the three adjustments, reset, and one-shot animation on the agreed local emulator, recording the actual package and test evidence. Keep coded Android UI verification in the separate Appium test project. Apple targets have not yet been validated.

The finished episode and a fully validated release are still in preparation.
