using Godot;
using System;

public class Level2 : Level
{
    private String[] _texts = new String[] {"You can rotate every StickyBlock, even the last one. \n\n LEFT CLICK to rotate it."};
    private Label _label;

    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];   
    }

}
