using Godot;

public class LevelNumberIcon : VBoxContainer
{
    private TextureRect _starsRect;
    public TextureRect StarsRect { get { return _starsRect; } }


    [Export]
    private string _levelNumber = "0";
    public string LevelNumber { get { return _levelNumber; } set { _levelNumber = value; } }

    private TextureButton _button;
    public TextureButton Button { get { return _button; } }

    private Color _modulateColor;
    private Color _offFocusColor;
    private GameManager _gameManager;
    private Label _label;

    public override void _Ready()
    {
        _gameManager = GetTree().Root.GetNode<GameManager>("Main/GameManager");
        _button = GetNode<TextureButton>("TextureButton");
        _label = GetNode<Label>("TextureButton/Label");
        _starsRect = GetNode<TextureRect>("CenterContainer/TextureRect");

        _button.Name = this.Name;
        _modulateColor = _button.SelfModulate;
        _offFocusColor = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b, 0.2f);
        _label.Text = _levelNumber;

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

