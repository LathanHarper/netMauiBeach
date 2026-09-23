namespace RadialSurfSchool;

public partial class MainPage : ContentPage
{
    private const string WaveAnimation = "LessonWave";

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnRideWaveClicked(object? sender, EventArgs e)
    {
        this.AbortAnimation(WaveAnimation);
        SetWaveRunning(true);
        ValueSlider.Value = 0;

        // Animate the same value the slider controls. The XAML bindings do the rest.
        new Animation(value => ValueSlider.Value = value, 0, 100)
            .Commit(this, WaveAnimation, length: 1800, easing: Easing.SinInOut,
                finished: (_, _) => SetWaveRunning(false));
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        this.AbortAnimation(WaveAnimation);
        SetWaveRunning(false);
        ValueSlider.Value = 60;
        StartSlider.Value = -120;
        SweepSlider.Value = 240;
    }

    private void SetWaveRunning(bool running)
    {
        RideButton.IsEnabled = !running;
        ValueSlider.IsEnabled = !running;
    }

    protected override void OnDisappearing()
    {
        this.AbortAnimation(WaveAnimation);
        SetWaveRunning(false);
        base.OnDisappearing();
    }
}
