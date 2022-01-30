using Godot;

public class LevelTypeMenu : MenuTemplates
{
    public override void _Ready()
    {
        base._Ready();
        VBoxContainer easy = (VBoxContainer)FindNode("EASY");
        TextureButton firstbutton = easy.GetNode<TextureButton>("CenterContainer/EASY");
        firstbutton.GrabFocus();
    }

    public override void ConnectButtons()
    {
        string nodeName = this.Name;
        string targetMethod = $"_on_{nodeName}_button_pressed";
        foreach (TextureButton button in _buttons)
        {
            button.Connect("pressed", GetTree().Root.GetChild(0), targetMethod, new Godot.Collections.Array { button.Name });
            button.Connect("mouse_exited", this, "_on_mouse_exited", new Godot.Collections.Array { button });
            button.Connect("mouse_entered", this, "_on_mouse_entered", new Godot.Collections.Array { button });
        }
    }
    public override void _on_mouse_entered(TextureButton button)
    {
        if (button.IsInGroup("hoverableButton"))
        {
            _modulate = button.SelfModulate;
            button.SelfModulate *= 1.1f;
            return;
        }
        button.GrabFocus();
    }
    public override void _on_mouse_exited(TextureButton button)
    {
        if (button.IsInGroup("hoverableButton"))
        {
            button.SelfModulate = _modulate;
            return;
        }

    }
}
