using Godot;

public class PlayerCamera : Camera2D
{
    private float _zoom_value = 1f;
    public float ZoomValue
    {
        get { return _zoom_value; }
        set { SetZoom(value); }
    }
    private Vector2 _zoom_limits = new Vector2(1f, 1f);
    private float _zoom_step = 0.4f;
    private float _zoom_animation_duration = 0.2f;
    public float ZoomAnimationDuration { get { return _zoom_animation_duration; } }
    public Vector2 ZoomLimits
    {
        get { return _zoom_limits; }
        set { _zoom_limits = value; }
    }
    private Tween _tween;
    public Tween TweenCamera
    {
        get { return _tween; }
    }

    private bool _zooming = false;

    private Vector2 _lastZoom;
    private Vector2 _lastOffset;


    [Signal]
    delegate void OffsetZoom(Vector2 newZoom, Vector2 oldZoom);

    public void BackToLastPosition()
    {
        Offset = _lastOffset;
        Zoom = _lastZoom;
        _zooming = false;
    }
    public void Reset()
    {
        Offset = Vector2.Zero;
        Zoom = Vector2.One;
        Current = true;
        _zoom_value = 1f;
        _zooming = false;
    }

    private void SetZoom(float value)
    {
        
        var viewport_size = GetViewport().Size;
        var previous_zoom = Zoom;
        _zoom_value = Mathf.Clamp(value, _zoom_limits[0], _zoom_limits[1]);
        var newZoom = new Vector2(_zoom_value, _zoom_value);

        EmitSignal(nameof(OffsetZoom), newZoom, previous_zoom);
       
        _tween.InterpolateProperty(
            this, "zoom", Zoom, newZoom, _zoom_animation_duration,
            Tween.TransitionType.Sine, Tween.EaseType.Out
            );

        _tween.Start();
        _zooming = true;
    }


    public override void _Ready()
    {
        _tween = GetNode<Tween>("TweenCamera");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!_zooming)
        {
            if (@event.IsActionPressed("zoom_in"))
            {
                ZoomValue = _zoom_value - _zoom_step*2;
            }
            if (@event.IsActionPressed("zoom_out"))
            {
                ZoomValue = _zoom_value + _zoom_step;
            }
        }

        @event.Dispose();
    }

    public void _on_TweenCamera_tween_completed(object _, NodePath __)
    {
        _zooming = false;
        _lastZoom = Zoom;
        _lastOffset = Offset;
    }


}
