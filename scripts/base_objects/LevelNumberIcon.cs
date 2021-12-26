using Godot;

public class LevelNumberIcon : VBoxContainer
{
    private TextureRect _starsRect;
    public TextureRect StarsRect {get { return _starsRect; }}
    public TextureButton Button { get { return _button; }}


    [Export]
    private Texture _texture;
    [Export]
    private string _levelNumber = "0";

    private TextureButton _button;
    private Color _modulateColor;
    private GameManager _gameManager;
    private Label _label;
   
    public override void _Ready()
    {
        _gameManager = GetTree().Root.GetNode<GameManager>("Main/GameManager");
        _button = GetNode<TextureButton>("TextureButton");
        _label = GetNode<Label>("TextureButton/Label");
        _starsRect = GetNode<TextureRect>("CenterContainer/TextureRect");
        
        _button.Name = this.Name;
        // _button.TextureNormal = _texture;
        _modulateColor = _button.SelfModulate;
        _label.Text = _levelNumber;

    }
    
    public void _on_TextureButton_focus_entered()
    {
        // _button.Modulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b);
        this.SelfModulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b);
    }
    public void _on_TextureButton_focus_exited()
    {
        // _button.Modulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b, 0.2f);
        this.SelfModulate = new Color(_modulateColor.r, _modulateColor.g, _modulateColor.b, 0.2f);
    }



}
    