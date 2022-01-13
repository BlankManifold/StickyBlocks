using Godot;
using System;

public class Level7 : Level
{
    private String[] _texts = new String[] {"The red block can also stick on the short-side of the StickyBlocks"};
    private Label _label;

    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];   
    }

}

