namespace RadialSurfSchool.Controls;

public sealed class RadialSegment : BindableObject
{
    private double _angle;
    private bool _isActive;

    public double Angle
    {
        get => _angle;
        set
        {
            if (_angle == value) return;
            _angle = value;
            OnPropertyChanged();
        }
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value) return;
            _isActive = value;
            OnPropertyChanged();
        }
    }
}
