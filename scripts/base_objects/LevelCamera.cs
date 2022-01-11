using Godot;

public class LevelCamera : Camera2D
{
    private bool _zooming = false;

    private float _zoomValue = 1f;
    public float ZoomValue { get { return _zoomValue; } set { SetZoom(value); } }

    private Vector2 _zoomLimits = new Vector2(1f, 1f);
    public Vector2 ZoomLimits { get { return _zoomLimits; } set { _zoomLimits = value; } }

    private float _zoomStep = 0.4f;
    public float ZoomStep { get { return _zoomStep; } set { _zoomStep = value; } }

    private float _zoomAnimationDuration = 0.2f;
    public float ZoomAnimationDuration { get { return _zoomAnimationDuration; } }

    private Tween _tween;
    public Tween TweenCamera { get { return _tween; } }

    private Vector2 _initialZoom;
    public Vector2 InitialZoom { get { return _initialZoom; } set { _initialZoom = value; } }


    private Vector2 _lastZoom = Vector2.One;
    private Vector2 _lastOffset = Vector2.Zero;


    [Signal]
    delegate void ChangeParent(bool followPlayer);


    [Signal]
    delegate void OffsetZoom(Vector2 newZoom, Vector2 oldZoom);

    public void BackToLastPosition()
    {
        Offset = _lastOffset;
        _zooming = false;
    }
    public void Reset()
    {
        Offset = Vector2.Zero;
        Zoom = _initialZoom;
        Current = true;
        _zoomValue = _initialZoom.x;
        _zooming = false;
    }

    private void SetZoom(float value)
    {
        var previous_zoom = Zoom;

        _zoomValue = Mathf.Clamp(value, _zoomLimits[0], _zoomLimits[1]);
        var newZoom = new Vector2(_zoomValue, _zoomValue);

        EmitSignal(nameof(OffsetZoom), newZoom, previous_zoom);

        _tween.InterpolateProperty(this, "zoom", Zoom, newZoom, _zoomAnimationDuration, Tween.TransitionType.Sine, Tween.EaseType.Out);
        _tween.Start();

        _zooming = true;
    }


    public void Init(float maxZoomValue, float initialZoomValue)
    {
        Zoom *= maxZoomValue;
        
        if (initialZoomValue != 0f)
        {
            Zoom *= (initialZoomValue / maxZoomValue);
        }

        _initialZoom = Zoom;
        _zoomLimits = new Vector2(1, maxZoomValue);
        _zoomStep = (maxZoomValue - 1) / 2;
        _zoomValue = maxZoomValue;
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
                if (Zoom.x > _zoomLimits.x)
                {
                    ZoomValue = _zoomValue - _zoomStep;
                }
            }
            if (@event.IsActionPressed("zoom_out"))
            {
                if (Zoom.x < _zoomLimits.y)
                {
                    ZoomValue = _zoomValue + _zoomStep;
                }
            }
        }

        @event.Dispose();
    }

    public void _on_TweenCamera_tween_completed(object _, NodePath __)
    {
        _zooming = false;
        _lastOffset = Offset;

        // if (ZoomLimits.y > 1f)
        // {
        //     if (Zoom.x == 1 && !_followPlayer)
        //     {
        //         EmitSignal(nameof(ChangeParent), true);
        //         _followPlayer = true;
        //         return;
        //     }

        //     if (Zoom.x != 1 && _followPlayer)
        //     {
        //         EmitSignal(nameof(ChangeParent), false);
        //         _followPlayer = false;

        //     }
        // }

    }


}

