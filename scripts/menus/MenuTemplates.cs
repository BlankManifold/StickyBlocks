using Godot;


public class MenuTemplates : Control
{
    protected Godot.Collections.Array<TextureButton> _buttons;
    protected GameManager _gameManager;
    protected Color _modulate;

    public override void _Ready()
    {
        _buttons = new Godot.Collections.Array<TextureButton>(GetTree().GetNodesInGroup("buttons"));
        _gameManager = GetParent().GetNode<GameManager>("GameManager");
    }

    public virtual void ConnectButtons()
    {
        string nodeName = this.Name;
        string targetMethod = $"_on_{nodeName}_button_pressed";
        foreach (TextureButton button in _buttons)
        {
            button.Connect("pressed", GetTree().Root.GetChild(0), targetMethod, new Godot.Collections.Array { button.Name });
            if (button.IsInGroup("hoverableButton"))
            {
                button.Connect("mouse_exited", this, "_on_mouse_exited", new Godot.Collections.Array { button });
                button.Connect("mouse_entered", this, "_on_mouse_entered", new Godot.Collections.Array { button });
            }
        }
    }

    public virtual void _on_mouse_entered(TextureButton button)
    {
        // _scale = button.RectScale;
        // button.RectScale *= 1.1f;
        _modulate = button.SelfModulate;
        button.SelfModulate *= 1.1f;
    }
    public virtual void _on_mouse_exited(TextureButton button)
    {
        // button.RectScale = _scale;
        button.SelfModulate = _modulate;
    }

}
