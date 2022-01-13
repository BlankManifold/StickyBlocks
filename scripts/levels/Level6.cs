using Godot;
using System;

public class Level6 : Level
{
    private String[] _texts = new String[] {"Manipulate the collision point of the red block on the StickyBlocks using the different angolations."};
    private Label _label;

    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];   
    }

}

