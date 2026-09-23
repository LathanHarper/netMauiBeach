namespace RadialSurfSchool.Controls;

public partial class RadialGauge : ContentView
{
    // A fixed count keeps the first lesson small. These are ordinary XAML Views.
    public const int SegmentCount = 13;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value), typeof(double), typeof(RadialGauge), 0d,
        propertyChanged: OnGaugeChanged,
        coerceValue: (_, value) => ClampFinite((double)value, 0, 100));

    public static readonly BindableProperty StartAngleProperty = BindableProperty.Create(
        nameof(StartAngle), typeof(double), typeof(RadialGauge), -120d,
        propertyChanged: OnGaugeChanged,
        coerceValue: (_, value) => double.IsFinite((double)value) ? value : 0d);

    public static readonly BindableProperty SweepAngleProperty = BindableProperty.Create(
        nameof(SweepAngle), typeof(double), typeof(RadialGauge), 240d,
        propertyChanged: OnGaugeChanged,
        coerceValue: (_, value) => ClampFinite((double)value, -360, 360));

    public IReadOnlyList<RadialSegment> Segments { get; } =
        Enumerable.Range(0, SegmentCount).Select(_ => new RadialSegment()).ToArray();

    public double Value
    {
        get => (double)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double StartAngle
    {
        get => (double)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public double SweepAngle
    {
        get => (double)GetValue(SweepAngleProperty);
        set => SetValue(SweepAngleProperty, value);
    }

    public RadialGauge()
    {
        InitializeComponent();
        UpdateSegments();
    }

    private static void OnGaugeChanged(BindableObject bindable, object oldValue, object newValue) =>
        ((RadialGauge)bindable).UpdateSegments();

    private static double ClampFinite(double value, double minimum, double maximum) =>
        double.IsFinite(value) ? Math.Clamp(value, minimum, maximum) : 0;

    private void UpdateSegments()
    {
        // Open arcs include both endpoints. A complete circle must not repeat its first piece.
        var intervals = Math.Abs(SweepAngle) == 360 ? SegmentCount : SegmentCount - 1;
        var step = SweepAngle / intervals;
        var activeCount = (int)Math.Round(Value / 100 * SegmentCount, MidpointRounding.AwayFromZero);

        for (var index = 0; index < SegmentCount; index++)
        {
            Segments[index].Angle = StartAngle + index * step;
            Segments[index].IsActive = index < activeCount;
        }
    }
}
