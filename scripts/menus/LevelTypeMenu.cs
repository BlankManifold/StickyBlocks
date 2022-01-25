using Godot;

public class LevelTypeMenu : MenuTemplates
{
    public override void _Ready()
    {   
        base._Ready();
        VBoxContainer easy = (VBoxContainer)FindNode("EASY");
        TextureButton firstbutton = (TextureButton)easy.FindNode("EASY");
        firstbutton.GrabFocus();
    }

}
