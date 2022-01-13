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

    private LevelCamera _cameraLevel;
    private Vector2 _offsetCamera = Vector2.Zero;
    private Vector2 _areaSize;
    private Vector2 _centerArea;

    private Tween _tween;

    [Export]
    protected float _intialZoom = 0f;

    [Signal]
    public delegate void GameCompleted(bool owned, int movesCounter);
    [Signal]
    public delegate void RotatingTweenCompleted();

    [Export]
    protected float _maxZoomConstraint = 0f;

    private float[] _levelData;

    private Label _label;

    private Dictionary<string, int> _bounds = new Dictionary<string, int> { { "bottom", 0 }, { "top", 0 }, { "right", 0 }, { "left", 0 } };

    public override void _Process(float _)
    {
        if (_playerBlock.Debug)
        {
            _label.Text = $"Offset: {_cameraLevel.Offset} \n PlayerPos: {_playerBlock.GlobalPosition} \n CameraPos: {_cameraLevel.GlobalPosition} \n CenterArea; {_centerArea} \n CenterCamera: {_cameraLevel.GetCameraScreenCenter()} \n Direction: {_playerBlock.GlobalPosition.DirectionTo(_centerArea)}";
        }

    }
    public override void _Ready()
    {

        _playerBlock = (PlayerBlock)FindNode("PlayerBlock");
        _background = GetNode<TextureRect>("Background/TextureRect");
        Node2D nodes = (Node2D)FindNode("RotationStickyBlocks");
        _allStickyBlocks = nodes.GetChildren();
        _tween = _playerBlock.GetNode<Tween>("Tween");

        _cameraLevel = (LevelCamera)FindNode("Camera2DLevel");
        _cameraLevel.Current = true;


        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            block.Connect(nameof(RotationStickyBlock.ImRotating), this, nameof(_on_RotationStickyBlock_ImRotating));
        }

        SetInitialCurrentBlock();
        _playerBlock.UpdateState(_currentStickyBlock);
        _playerBlock.CurrentBlock = _currentStickyBlock;
        _currentStickyBlock.UpdateState();

        SetLabels();
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

    
    private void SetLevelData()
    {
        _levelData = new float[_allStickyBlocks.Count];

        int i = 0;
        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            _levelData[i] = block.GlobalRotation;
            block.UpdateState();
            i++;
        }
    }
    private void BackOneMove()
    {
        _playerBlock.BackOneMove();
        _currentStickyBlock.IsCurrentBlock = false;
        RotationStickyBlock lastBlock = (RotationStickyBlock)_playerBlock.LastState["Block"];
        lastBlock.IsCurrentBlock = true;
        _currentStickyBlock = lastBlock;
        _playerBlock.CurrentBlock = lastBlock;

        _currentStickyBlock.BackOneMove();
    }
    protected void ResetLevel()
    {
        _movesCounter = 0;
        _movesLabel.Text = $"Moves: 0";

        _currentStickyBlock.IsCurrentBlock = false;
        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");
        initialBlock.IsCurrentBlock = true;
        _currentStickyBlock = initialBlock;

        _playerBlock.Reset();
        _playerBlock.CurrentBlock = _currentStickyBlock;
        _playerBlock.UpdateBlock(_currentStickyBlock);
        // _camera.Reset();
        _cameraLevel.Reset();

        int i = 0;
        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            block.IsRotated = false;
            block.GlobalRotation = _levelData[i];
            i++;
        }
    }
    private void SetLabels()
    {
        Label levelLabel = (Label)FindNode("Level");
        levelLabel.Text = $"Level: {_number}";
    }
    private void SetCameraLimits()
    {
        CollisionShape2D shape = (CollisionShape2D)FindNode("CollisionShape2D");
        RectangleShape2D rect = (RectangleShape2D)shape.Shape;

        Vector2 center = shape.Position;
        _centerArea = center;

        _bounds["top"] = (int)(center[1] - rect.Extents[1]); ;
        _bounds["bottom"] = (int)(center[1] + rect.Extents[1]);
        _bounds["left"] = (int)(center[0] - rect.Extents[0]); ;
        _bounds["right"] = (int)(center[0] + rect.Extents[0]);

        Vector2 size = GetViewport().Size;
        _areaSize = rect.Extents * 2;

        if (_maxZoomConstraint != 0)
        {
            _cameraLevel.Init(_maxZoomConstraint, _intialZoom);
            return;
        }

        float maxLevelZoomValue = Mathf.Max(_areaSize.y / size.y, _areaSize.x / size.x) + 0.25f;
        _cameraLevel.Init(maxLevelZoomValue, _intialZoom);

    }

    protected void SetInitialCurrentBlock()
    {
        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");
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

    }
    public async void _on_RotationStickyBlock_ImRotating()
    {
        if (!_playerBlock.Moving && !_playerBlock.Rotating)
        {
            UpdatePlayerPosition();
            if (_playerBlock.Debug)
            {
                await ToSignal(_tween, "tween_completed");
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
        _currentStickyBlock.UpdateState();
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
    public void _on_HUDbuttons_UndoPressed()
    {
        BackOneMove();
    }  
    public void _on_Area2D_body_exited(Node body)
    {
        if (!_currentStickyBlock.IsLast)
        {

            _playerBlock.BackToLastPosition();

            _currentStickyBlock.IsCurrentBlock = true;
            RotationStickyBlock.CanRotate = true;
        }
    }
    public void _on_PlayerBlock_AddLine(Vector2 pivot, Vector2 normal)
    {
        UpdateLine(pivot, normal);
    }
   
    public void _on_Camera2DLevel_OffsetZoom(Vector2 newZoom, Vector2 oldZoom)
    {
        Vector2 centerCamera = _cameraLevel.GetCameraScreenCenter();
        Vector2 direction = centerCamera.DirectionTo(_playerBlock.GlobalPosition);
        float distance = _playerBlock.GlobalPosition.DistanceTo(centerCamera);

        Vector2 oldOffset = _cameraLevel.Offset;
        Vector2 newOffset = oldOffset - direction * (newZoom - oldZoom) * distance;

        if (newZoom.x - oldZoom.x > 0)
        {
            newOffset = newOffset * (_cameraLevel.ZoomLimits.y / newZoom.x - 1);
        }

        Tween tweenCamera = _cameraLevel.TweenCamera;
        float duration = _cameraLevel.ZoomAnimationDuration;


        tweenCamera.InterpolateProperty(_cameraLevel, "offset", oldOffset, newOffset, duration,
            Tween.TransitionType.Sine, Tween.EaseType.Out);
        tweenCamera.Start();
    }

    public void UpdateLine(Vector2 pivot, Vector2 normal)
    {
        _line.ClearPoints();
        _line.AddPoint(pivot);
        _line.AddPoint(pivot + normal * 2000);
    }
    public void _on_Tween_tween_completed(object _, NodePath __)
    {
        EmitSignal(nameof(RotatingTweenCompleted));
    }

}









	









