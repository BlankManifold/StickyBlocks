using Godot;

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
    protected int[] _movesRequired;

    [Export]
    protected int _number;
    public int Number { get { return _number; } set { _number = value; } }

    private Line2D _line;
    private Camera2D _camera;

    [Signal]
    public delegate void GameCompleted(int stars, int movesCounter);

    private float[] _levelData;


    public override void _Ready()
    {
        // _playerBlock = GetNode<PlayerBlock>("Blocks/Control/PlayerBlock");
        // _playerBlock = GetNode<PlayerBlock>("PlayerBlock");
        _playerBlock = (PlayerBlock)FindNode("PlayerBlock");
        _background = GetNode<TextureRect>("Background/TextureRect");
        Node2D nodes = (Node2D)FindNode("RotationStickyBlocks");
        _allStickyBlocks = nodes.GetChildren();
        _camera = _playerBlock.GetNode<Camera2D>("PlayerCamera");
        // _allStickyBlocks = GetNode<Control>("RotationStickyBlocks").GetChildren();
        // _allStickyBlocks = GetNode<Node2D>("Blocks/Control/RotationStickyBlocks").GetChildren();

        foreach (RotationStickyBlock block in _allStickyBlocks)
        {
            block.Connect(nameof(RotationStickyBlock.ImRotating), this, nameof(_on_RotationStickyBlock_ImRotating));
        }
        // SetInitialPosition(GetStartPosition());

        SetInitialCurrentBlock();
        SetStarsLabel();

        _movesLabel = (Label)FindNode("Moves");

        Area2D area = GetNode<Area2D>("Area2D");
        area.Connect("body_exited", this, nameof(_on_Area2D_body_exited));

        SetCameraLimits();

        if (_playerBlock.Debug)
        {
            // Control control = GetNode<Control>("Blocks/Control");
            _line = new Line2D();
            // control.AddChild(_line);
            AddChild(_line);
            _line.Width = 5;
            UpdateLine(_playerBlock.GlobalPosition, _playerBlock.DashDirection);
        }

        SetLevelData();

        // RotationStickyBlock finalBlock = (RotationStickyBlock)FindNode("FinalBlock");
        // TextureRect littleBlock = GetNode<TextureRect>("TextureRect");
        // littleBlock.RectPosition = new Vector2(1,-138);

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
        stars1Label.Text = _movesRequired[2].ToString();
        Label stars2Label = (Label)FindNode("stars2Label");
        stars2Label.Text = _movesRequired[1].ToString();
        Label stars3Label = (Label)FindNode("stars3Label");
        stars3Label.Text = _movesRequired[0].ToString();

    }
    private void SetCameraLimits()
    {
        CollisionShape2D shape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        RectangleShape2D rect = (RectangleShape2D)shape.Shape;
        _camera = _playerBlock.GetNode<Camera2D>("PlayerCamera");

        Vector2 center = shape.Position;
        Vector2 extents = rect.Extents;

        int border = 20;

        int top = (int)(center[1] - extents[1]) + border;
        int bottom = (int)(center[1] + extents[1]) - border;
        int left = (int)(center[0] - extents[0]) + border;
        int right = (int)(center[0] + extents[0]) - border;

        _camera.LimitBottom = bottom;
        _camera.LimitTop = top;
        _camera.LimitLeft = left;
        _camera.LimitRight = right;

    }
    protected void SetInitialPosition(Vector2 initialPosition) => _playerBlock.GlobalPosition = initialPosition;
    protected Vector2 GetStartPosition()
    {
        Position2D initialPosition = GetNode<Position2D>("Position2D");
        return initialPosition.GlobalPosition + _playerBlock.Offset;
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
        _playerBlock.GlobalPosition = pivot + (_playerBlock.GlobalPosition -pivot).Rotated(newAngle);
        _auxAngle = angle;

    }
    public void RotateAround(float angle)
    {
        Tween tween = _playerBlock.GetNode<Tween>("Tween");
        
        // tween.InterpolateProperty(this, "position:x", initialPosition.x, finalPosition.x, 0.3f,Tween.TransitionType.Sine);
        // tween.InterpolateProperty(this, "position:y", initialPosition.y, finalPosition.y, 0.3f,Tween.TransitionType.Sine);
        //tween.InterpolateProperty(this, "rotation", Rotation, Rotation+angle, 0.3f);
        
        tween.InterpolateMethod(this, "CircularMotion",0, angle, 0.3f);
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

        int stars = 0;
        if (movesCounter <= _movesRequired[0])
        {
            stars = 3;
        }
        else if (movesCounter <= _movesRequired[1])
        {
            stars = 2;
        }
        else if (movesCounter <= _movesRequired[2])
        {
            stars = 1;
        }

        EmitSignal(nameof(GameCompleted), stars, movesCounter);
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
        _playerBlock.BackToLastPosition();
        // _currentStickyBlock.BackToLastRotation();
        _currentStickyBlock.IsCurrentBlock = true;
        RotationStickyBlock.CanRotate = true;

        _camera.Align();
    }
    public void _on_PlayerBlock_AddLine(Vector2 pivot, Vector2 normal)
    {
        UpdateLine(pivot, normal);
    }
    public void UpdateLine(Vector2 pivot, Vector2 normal)
    {
        _line.ClearPoints();
        _line.AddPoint(pivot);
        _line.AddPoint(pivot + normal * 2000);
    }
}











