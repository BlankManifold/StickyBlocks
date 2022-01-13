using Godot;
using System;

public class Level3 : Level
{
    private String[] _texts = new String[] {"Lighter the color smaller the rotation angle. Possible rotation angle are in degree: 0°, 30°, 45°, 60° 90°, 180°."};
    private Label _label;

    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];   
    }

}
