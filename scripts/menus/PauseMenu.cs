using Godot;

public class PauseMenu : Popup
{
    private Godot.Collections.Array<TextureButton> _buttons;
    private GameManager _gameManager;

    private Color _modulate;

    public override void _Ready()
    {
        _buttons = new Godot.Collections.Array<TextureButton>(GetTree().GetNodesInGroup("buttons"));
        _gameManager = GetTree().Root.GetNode<GameManager>("Main/GameManager");
        ConnectButtons();
    }
    public void ConnectButtons()
    {
        string targetMethod = "_on_PauseMenu_button_pressed";
        foreach (TextureButton button in _buttons)
        {
            button.Connect("pressed", _gameManager, targetMethod, new Godot.Collections.Array { button.Name, this });
            if (button.IsInGroup("hoverableButton"))
            {
                button.Connect("mouse_exited", this, "_on_mouse_exited", new Godot.Collections.Array { button });
                button.Connect("mouse_entered", this, "_on_mouse_entered", new Godot.Collections.Array { button });
            }
        }
    }

    public virtual void _on_mouse_entered(TextureButton button)
    {
        _modulate = button.SelfModulate;
        button.SelfModulate *= 1.1f;
    }
    public virtual void _on_mouse_exited(TextureButton button)
    {
        button.SelfModulate = _modulate;
    }


}
