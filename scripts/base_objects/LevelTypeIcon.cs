using Godot;


public class LevelTypeIcon : VBoxContainer
{
    [Export]
    protected Texture _texture;

    protected TextureButton _button;

    protected TextureRect _lockedRect;

    private AnimationPlayer _animationPlayer;
    
    private Label _label;
    private Label _typeLabel;

    private Color _modulateColor;
    private Color _offFocusColor;
    protected GameManager _gameManager;
    private bool _isUnlocked = false;
    public bool IsUnlocked { get { return _isUnlocked; } set { _isUnlocked = value; } }


    [Signal]
    delegate void JustUnLocked();
    public async override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _gameManager = GetTree().Root.GetNode<GameManager>("Main/GameManager");
        
        _button = GetNode<TextureButton>("CenterContainer/TextureButton");
        _button.Name = this.Name;
        _button.TextureNormal = _texture;
        _modulateColor = _button.SelfModulate;
        _offFocusColor = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b, 0.2f);
        
        _label = GetNode<Label>("Label");
        _label.Text = $"{_gameManager.NumberOfCompleted(Name)}/{_gameManager.NumberOfLevel(Name)}";
                
        _typeLabel = _button.GetNode<Label>("Label");
        _typeLabel.Text = this.Name;
        

        if (_gameManager.JustUnLocked[Name])
        {
            _gameManager.JustUnLocked[Name] = false;
            await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
            _gameManager.SetGlowStrength(1.35f);
            _animationPlayer.Play("unlock");
            await ToSignal(_animationPlayer,"animation_finished");
            _gameManager.SetGlowStrength(1f);
        }
        
        
        _lockedRect = GetNode<TextureRect>("CenterContainer/LockedRect");
        _isUnlocked = _gameManager.IsLevelUnlocked(_button.Name);
        
        if (_isUnlocked)
        {
           _lockedRect.Hide();
        }

        
        if (_button.HasFocus())
        {
            return;
        }
        _button.SelfModulate = _offFocusColor;

    }
    public void _on_TextureButton_focus_entered()
    {
        _button.SelfModulate = _modulateColor;
    }
    public void _on_TextureButton_focus_exited()
    {
        _button.SelfModulate = _offFocusColor;
    }



}


