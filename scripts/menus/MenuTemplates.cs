using Godot;


public class MenuTemplates : Control
{
    protected Godot.Collections.Array<TextureButton> _buttons;
    protected GameManager _gameManager;

 
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
        }


    }


}
