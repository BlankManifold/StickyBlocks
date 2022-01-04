using Godot;
using System.Collections.Generic;

public class Level : Node2D
{
    protected PlayerBlock _playerBlock;
    protected TextureRect _background;

    [Export]
    protected string _type = "Easy";
    public string Type { get { return _type; } }
    protected RotationStickyBlock _currentStickyBlock;
    protected Godot.Collections.Array _allStickyBlocks;

    private Label _movesLabel;
    private int _movesCounter = 0;

    private float _auxAngle = 0f;


    [Export]
    protected int _movesRequired;

    [Export]
    protected int _number;
    public int Number { get { return _number; } set { _number = value; } }

    private Line2D _line;
    private PlayerCamera _camera;
    private Vector2 _offsetCamera = Vector2.Zero;
    private Vector2 _areaSize;
    private Vector2 _centerArea;

    [Signal]
    public delegate void GameCompleted(bool owned, int movesCounter);

    private float[] _levelData;

    private Label _label;

    private Dictionary<string,int> _bounds = new Dictionary<string,int> {{"bottom",0}, {"top",0}, {"right",0},{"left",0} };


    public override void _Process(float _)
    {
        if (_playerBlock.Debug)
        {
            _label.Text = $"Offset: {_camera.Offset} \n PlayerPos: {_playerBlock.GlobalPosition} \n CameraPos: {_camera.Position} \n CenterArea; {_centerArea} \n CenterCamera: {_camera.GetCameraScreenCenter()} \n Direction: {_playerBlock.GlobalPosition.DirectionTo(_centerArea)}";
        }

        if (IsOutOfBounds())
        {
            _camera.Current = false;
        }

    }
    public override void _Ready()
    {

        // _playerBlock = GetNode<PlayerBlock>("PlayerBlock");
        _playerBlock = (PlayerBlock)FindNode("PlayerBlock");
        _background = GetNode<TextureRect>("Background/TextureRect");
        Node2D nodes = (Node2D)FindNode("RotationStickyBlocks");
        _allStickyBlocks = nodes.GetChildren();
        _camera = _playerBlock.GetNode<PlayerCamera>("PlayerCamera");


        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            block.Connect(nameof(RotationStickyBlock.ImRotating), this, nameof(_on_RotationStickyBlock_ImRotating));
        }

        SetInitialCurrentBlock();
        SetStarsLabel();

        _movesLabel = (Label)FindNode("Moves");

        Area2D area = (Area2D)FindNode("Area2D");
        area.Connect("body_exited", this, nameof(_on_Area2D_body_exited));

        SetCameraLimits();

        if (_playerBlock.Debug)
        {
            CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
            _label = new Label();
            _line = new Line2D();
            AddChild(_line);
            layer.AddChild(_label);
            _line.Width = 5;
            UpdateLine(_playerBlock.GlobalPosition, _playerBlock.DashDirection);
        }

        SetLevelData();
    }

    private bool IsOutOfBounds()
    {
        Vector2 pos = _playerBlock.GlobalPosition;
        float border = 50;
        return (pos.x > _bounds["right"] - border  || pos.x < _bounds["left"] + border 
                || pos.y < _bounds["top"] + border || pos.y > _bounds["bottom"] - border);
    }


    private void SetLevelData()
    {
        _levelData = new float[_allStickyBlocks.Count];

        int i = 0;
        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            _levelData[i] = block.GlobalRotation;
            i++;
        }
    }
    private void ResetLevel()
    {
        _movesCounter = 0;
        _movesLabel.Text = $"Moves: 0";

        _currentStickyBlock.IsCurrentBlock = false;
        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");
        initialBlock.IsCurrentBlock = true;
        _currentStickyBlock = initialBlock;

        _playerBlock.Reset();
        _camera.Reset();

        int i = 0;
        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            block.IsRotated = false;
            block.GlobalRotation = _levelData[i];
            i++;
        }
    }
    private void SetStarsLabel()
    {
        Label levelLabel = (Label)FindNode("Level");
        levelLabel.Text = $"Level: {_number}";

        Label stars1Label = (Label)FindNode("stars1Label");
        stars1Label.Text = _movesRequired.ToString();

    }
    private void SetCameraLimits()
    {
        CollisionShape2D shape = (CollisionShape2D)FindNode("CollisionShape2D");
        RectangleShape2D rect = (RectangleShape2D)shape.Shape;

        Vector2 center = shape.Position;
        _centerArea = center;
        Vector2 extents = rect.Extents;
        _areaSize = extents * 2;

        _bounds["top"] = (int)(center[1] - extents[1]);;
        _bounds["bottom"] = (int)(center[1] + extents[1]);
        _bounds["left"] = (int)(center[0] - extents[0]);;
        _bounds["right"] = (int)(center[0] + extents[0]);
 
        Vector2 size = GetViewport().Size;
        _camera.ZoomLimits = new Vector2(1, Mathf.Max(_areaSize.y / size.y, _areaSize.x / size.x));

    }

    protected void SetInitialCurrentBlock()
    {
        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");
        // RotationStickyBlock initialBlock = GetNode<RotationStickyBlock>("RotationStickyBlocks/InitialBlock");
        // RotationStickyBlock initialBlock = GetNode<RotationStickyBlock>("Blocks/Control/RotationStickyBlocks/InitialBlock");
        initialBlock.IsCurrentBlock = true;
        _currentStickyBlock = initialBlock;
    }

    public void CircularMotion(float angle)
    {
        float newAngle = angle - _auxAngle;
        Vector2 pivot = _currentStickyBlock.GlobalPosition;
        _playerBlock.Rotate(newAngle);
        _playerBlock.GlobalPosition = pivot + (_playerBlock.GlobalPosition - pivot).Rotated(newAngle);
        _auxAngle = angle;

    }
    public void RotateAround(float angle)
    {
        Tween tween = _playerBlock.GetNode<Tween>("Tween");

        // tween.InterpolateProperty(this, "position:x", initialPosition.x, finalPosition.x, 0.3f,Tween.TransitionType.Sine);
        // tween.InterpolateProperty(this, "position:y", initialPosition.y, finalPosition.y, 0.3f,Tween.TransitionType.Sine);
        //tween.InterpolateProperty(this, "rotation", Rotation, Rotation+angle, 0.3f);

        tween.InterpolateMethod(this, "CircularMotion", 0, angle, 0.3f);
        tween.Start();

    }
    protected void UpdatePlayerPosition()
    {
        _auxAngle = 0f;
        float angle = _currentStickyBlock.RotationAngle;

        if (_currentStickyBlock.IsRotated)
        {
            angle = -angle;
        }

        // _playerBlock.Rotate(angle);
        // _playerBlock.GlobalPosition = _currentStickyBlock.GlobalPosition + (_playerBlock.GlobalPosition - _currentStickyBlock.GlobalPosition).Rotated(angle);
        RotateAround(angle);

        _playerBlock.DashDirection = _playerBlock.DashDirection.Rotated(angle);



    }
    public void ScaleModulate(float scale, bool down = true)
    {
        if (!down)
        {
            scale = 1 / scale;
        }

        Modulate = scale * Modulate;
        _background.Modulate = scale * _background.Modulate;
    }

    public void _on_PlayerBlock_ChangeStickyBlock(RotationStickyBlock newStickyBlock)
    {
        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            block.IsCurrentBlock = false;
        }

        newStickyBlock.IsCurrentBlock = true;
        _currentStickyBlock = newStickyBlock;
        RotationStickyBlock.CanRotate = true;

        // _camera.CallDeferred("align");



    }
    public void _on_RotationStickyBlock_ImRotating()
    {
        if (!_playerBlock.Moving && !_playerBlock.Rotating)
        {
            UpdatePlayerPosition();
            if (_playerBlock.Debug)
            {
                UpdateLine(_playerBlock.GlobalPosition, _playerBlock.DashDirection);
            }
        }
    }
    public void _on_PlayerBlock_ImOnLast(int movesCounter)
    {

        bool owned = false;
        if (movesCounter <= _movesRequired)
        {
            owned = true;
        }

        EmitSignal(nameof(GameCompleted), owned, movesCounter);
    }
    public void _on_PlayerBlock_MoveMade()
    {
        RotationStickyBlock.CanRotate = false;
        _currentStickyBlock.IsCurrentBlock = false;
        _movesCounter += 1;
        _movesLabel.Text = $"Moves: {_movesCounter}";
    }
    public void _on_HUDbuttons_PausePressed()
    {
        GetTree().Paused = true;
        ScaleModulate(0.75f);
        GetNode<Control>("ButtonsLayers/PauseMenu").Show();
    }
    public void _on_HUDbuttons_ResetPressed()
    {
        ResetLevel();
    }
    public void _on_Area2D_body_exited(Node body)
    {
        if (!_currentStickyBlock.IsLast)
        {
            _playerBlock.BackToLastPosition();
            _currentStickyBlock.IsCurrentBlock = true;
            RotationStickyBlock.CanRotate = true;
            _camera.Current = true;
            // _camera.CallDeferred("align");
            _camera.BackToLastPosition();
        }
    }
    public void _on_PlayerBlock_AddLine(Vector2 pivot, Vector2 normal)
    {
        UpdateLine(pivot, normal);
    }
    public void _on_PlayerBlock_OffsetZoom(Vector2 newZoom, Vector2 oldZoom)
    {

        Vector2 direction = _playerBlock.GlobalPosition.DirectionTo(_centerArea);
        Vector2 centerCamera = _camera.GetCameraScreenCenter();
        Vector2 oldOffset = _camera.Offset;
        Vector2 size = GetViewport().Size;


        
        Vector2 newOffset = oldOffset + direction * (_centerArea.DistanceTo(centerCamera)) * (newZoom-oldZoom);

        if (newZoom.x-oldZoom.x < 0)
        {
            // direction = -direction;
           newOffset = newOffset * (1- 1/newZoom.x);  
        }

        Tween tweenCamera = _camera.TweenCamera;
        float duration = _camera.ZoomAnimationDuration;
        
        tweenCamera.InterpolateProperty(_camera, "offset", oldOffset, newOffset, duration,
            Tween.TransitionType.Sine, Tween.EaseType.Out);
        tweenCamera.Start();
    }

    public void UpdateLine(Vector2 pivot, Vector2 normal)
    {
        _line.ClearPoints();
        _line.AddPoint(pivot);
        _line.AddPoint(pivot + normal * 2000);
    }
}














