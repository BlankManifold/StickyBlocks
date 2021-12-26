using Godot;

public class PlayerCamera : Camera2D
{
    private float _zoom_value;
    public float ZoomValue
    {
        get { return _zoom_value; }
        set { SetZoom(value); }
    }
    private Vector2 _zoom_limits = new Vector2(1f, 1f);
    private float _zoom_step = 0.1f;
    private float _zoom_animation_duration = 0.2f;
    public Vector2 ZoomLimits
    {
        get { return _zoom_limits; }
        set { _zoom_limits = value; }
    }
    private Tween _tween;

    private void SetZoom(float value)
    {
        _zoom_value = Mathf.Clamp(value, _zoom_limits[0], _zoom_limits[1]);

        _tween.InterpolateProperty(
            this, "zoom", Zoom, new Vector2(_zoom_value, _zoom_value), _zoom_animation_duration,
            Tween.TransitionType.Sine, Tween.EaseType.Out
            );

        _tween.Start();
    }
    public override void _Ready()
    {
        _tween = GetNode<Tween>("Tween");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("zoom_in"))
        {
            ZoomValue = _zoom_value - _zoom_step;
        }
        if (@event.IsActionPressed("zoom_out"))
        {
            ZoomValue = _zoom_value + _zoom_step;
        }
        @event.Dispose();
    }

}
