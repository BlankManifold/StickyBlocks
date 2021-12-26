using Godot;

public class PauseMenu : MenuTemplates
{
     public override void _Ready()
    {   
        _buttons = new Godot.Collections.Array<TextureButton>(GetTree().GetNodesInGroup("buttons"));
        _gameManager = GetTree().Root.GetNode<GameManager>("Main/GameManager");
        ConnectButtons();
    }
    public override void ConnectButtons()
    {
        string targetMethod = "_on_PauseMenu_button_pressed";
        foreach (TextureButton button in _buttons)
        {
            button.Connect("pressed", _gameManager, targetMethod, new Godot.Collections.Array { button.Name, this });
        }
    }

}
