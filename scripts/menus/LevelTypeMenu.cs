using Godot;

public class LevelTypeMenu : MenuTemplates
{
    public override void _Ready()
    {   
        base._Ready();
        _buttons[0].GrabFocus();
    }

}
