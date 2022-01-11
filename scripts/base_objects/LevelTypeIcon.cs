using Godot;


public class LevelTypeIcon : VBoxContainer
{
    [Export]
    protected Texture _texture;

    [Export]
    protected Texture _lockedTexture;
    protected TextureButton _button;

    protected TextureRect _lockedRect;

    private AnimationPlayer _animationPlayer;
    
    private Label _label;
    private Label _typeLabel;

    private Color _modulateColor;
    protected GameManager _gameManager;
    private bool _isUnlocked = false;
    public bool IsUnlocked { get { return _isUnlocked; } set { _isUnlocked = value; } }


    public async override void _Ready()
    {
        _gameManager = GetTree().Root.GetNode<GameManager>("Main/GameManager");
        _button = GetNode<TextureButton>("CenterContainer/TextureButton");
        _label = GetNode<Label>("Label");
        _lockedRect = GetNode<TextureRect>("CenterContainer/LockedRect");
        _typeLabel = _button.GetNode<Label>("Label");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        _button.Name = this.Name;
        _typeLabel.Text = this.Name;
        _modulateColor = _button.SelfModulate;

        _isUnlocked = _gameManager.IsLevelUnlocked(_button.Name);
        _button.TextureNormal = _texture;

        if (_gameManager.JustUnLocked[Name])
        {
            _gameManager.JustUnLocked[Name] = false;
            await ToSignal(GetTree().CreateTimer(1f), "timeout");
            _animationPlayer.Play("unlock");
            await ToSignal(_animationPlayer,"animation_finished");
        }
        
        
        if (_isUnlocked)
        {
           _lockedRect.Hide();
        }

        _label.Text = $"{_gameManager.NumberOfCompleted(Name)}/{_gameManager.NumberOfLevel(Name)}";
        
        if (_button.HasFocus())
        {
        _button.SelfModulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b);
        }
        _button.SelfModulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b, 0.2f);

    }
    public void _on_TextureButton_focus_entered()
    {
        _button.SelfModulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b);
    }
    public void _on_TextureButton_focus_exited()
    {
        _button.SelfModulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b, 0.2f);
    }



}


