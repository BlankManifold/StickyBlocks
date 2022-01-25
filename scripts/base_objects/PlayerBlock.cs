using Godot;


public class PlayerBlock : KinematicBody2D
{
    [Export]
    public bool Debug = false;

    private int _speed = 12;
    private bool _moving = false;
    private bool _rotating = false;
    public bool Moving { get { return _moving; } }
    public bool Rotating
    {
        get { return _rotating; }
        set { _rotating = value; }
    }
    // private bool _isMouseOver = false;
    private Vector2 _dashDirection = new Vector2(1, 0);
    private bool _isSelected = true;
    private int _movesCounter = 0;
    // private PlayerCamera _camera;

    private Vector2 _lastStationaryPosition;
    public Vector2 LastStationaryPosition { get { return _lastStationaryPosition; } }
    private Godot.Collections.Dictionary _lastState = new Godot.Collections.Dictionary() { { "Transform", null }, { "DashDirection", null }, { "Block", null } };
    public Godot.Collections.Dictionary LastState { get { return _lastState; } }

    private RotationStickyBlock _currentBlock;
    public RotationStickyBlock CurrentBlock { get { return _currentBlock;} set { _currentBlock = value; }}

    [Export]
    public Texture selectedTexture;
    [Export]
    public Texture unselectedTexture;

    public Vector2 DashDirection
    {
        get { return _dashDirection; }
        set { _dashDirection = value; }
    }

    private Vector2 _initialPosition;
    private Vector2 _initialDirection;
    private float _initialRotation;


    [Signal]
    delegate void ChangeStickyBlock(RotationStickyBlock newStickyBlock);

    [Signal]
    delegate void AddLine(Vector2 pivot, Vector2 normal);

    [Signal]
    delegate void ImOnLast(int movesCounter);

    [Signal]
    delegate void MoveMade();


    public override void _Ready()
    {
        RayCast2D ray = GetNode<RayCast2D>("RayCast2D");
        _initialDirection = ray.CastTo.Rotated(GlobalRotation).Normalized();
        _dashDirection = _initialDirection;

        _initialPosition = GlobalPosition;
        _initialRotation = GlobalRotation;

    }

    public override void _Process(float delta)
    {
        if (_moving)
        {
            MoveAndDirection();
        }
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("leave_block"))
        {
            if (_isSelected && !_moving && !_rotating)
            {
                _lastStationaryPosition = GlobalPosition;
                
                UpdateTransform();
                UpdateBlock(_currentBlock);

                _moving = true;
                _movesCounter++;
                
                EmitSignal(nameof(MoveMade));
            }
            inputEvent.Dispose();
            return;
        }

        inputEvent.Dispose();

    }

    public void BackOneMove()
    {
        GlobalTransform = (Transform2D)_lastState["Transform"];
        _dashDirection = (Vector2)_lastState["DashDirection"];
    }
    public void Reset()
    {
        GlobalPosition = _initialPosition;
        GlobalRotation = _initialRotation;
        _moving = false;
        _movesCounter = 0;
        _dashDirection = _initialDirection;
    }
    public void BackToLastPosition()
    {
        _moving = false;
        GlobalPosition = _lastStationaryPosition;
    }

    public void UpdateState(RotationStickyBlock block = null)
    {
        _lastState["Transform"] = GlobalTransform;
        _lastState["DashDirection"] = _dashDirection;
        _lastState["Block"] = block;
    }
    public void UpdateBlock(RotationStickyBlock block)
    {
        _lastState["Block"] = block;
    }
    public void UpdateTransform()
    {
        _lastState["Transform"] = GlobalTransform;
        _lastState["DashDirection"] = _dashDirection;
    }

    private void MoveAndDirection()
    {

        KinematicCollision2D collisionInfo = MoveAndCollide(_dashDirection * _speed);
        if (collisionInfo != null)
        {
            _moving = false;
            RotationStickyBlock collider = (RotationStickyBlock)collisionInfo.Collider;
            Vector2 normal = collisionInfo.Normal;
            Vector2 pivot = collisionInfo.Position;

            if (Debug)
            {
                EmitSignal(nameof(AddLine), pivot, normal);
            }

            if (collider.IsLast)
            {
                EmitSignal(nameof(ImOnLast), _movesCounter);
            }

            CollideAndRotate(normal, pivot);

            EmitSignal(nameof(ChangeStickyBlock), collider);
            _currentBlock = collider;

        }
    }

    private void CollideAndRotate(Vector2 normal, Vector2 pivot)
    {
        float cross = normal.Cross(_dashDirection);
        if (Mathf.Abs(cross) < 0.01f || Mathf.Abs(Mathf.Abs(cross) - 1f) < 0.01f)
        {
            _dashDirection = normal;
            return;
        }

        float angle = Mathf.Pi - Mathf.Acos(normal.Dot(_dashDirection));
        if (cross < -0.01f)
        {
            angle = -angle;
        }

        GlobalPosition = pivot + (GlobalPosition - pivot).Rotated(angle);
        Rotate(angle);

        _dashDirection = normal;

    }

    public void _on_Tween_tween_started(object _, NodePath __)
    {
        _rotating = true;
    }
    public void _on_Tween_tween_completed(object _, NodePath __)
    {
        _rotating = false;
    }


    [Signal]
    delegate void OffsetZoom(Vector2 newZoom, Vector2 oldZoom);

    public void _on_PlayerCamera_OffsetZoom(Vector2 newZoom, Vector2 oldZoom)
    {
        EmitSignal(nameof(OffsetZoom), newZoom, oldZoom);
    }

}


