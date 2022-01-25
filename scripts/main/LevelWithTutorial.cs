using Godot;
using System;

public class LevelWithTutorial : Level
{
    [Export(PropertyHint.MultilineText)]
    protected String _text0;

    protected Label _label;

    public override void _Ready()
    {
        base._Ready();

        _gotoAnim = "gotoLevelsWithTutorial";
        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _text0;   
    }

    public override void ScaleModulate(bool down = true)
    {
        if (down)
        {
           _animationPlayerLast.Play("pausedWithTutorial");
           return;
        }
        
        _animationPlayerLast.Play("pausedResetWithTutorial");

    }

}

