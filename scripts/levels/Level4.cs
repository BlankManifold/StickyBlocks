using Godot;
using System;

public class Level4 : Level
{
    private String[] _texts = new String[] {"In some level you might want to zoom-in or zoom-out the camera. \n\n Press SPACEBAR to continue", 
                                            "Use the MOUSE-WHEEL to handle the zoom" };
    private Label _label;
    private InputPausedAnimation _animationPlayer;

    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];

        _animationPlayer = (InputPausedAnimation)FindNode("AnimationPlayer");
        _animationPlayer.PlayBackAndForth = true;
        _animationPlayer.Play("tutorial");
        
        GetTree().Paused = true;
    }
    public void _on_AnimationPlayer_ActionPressed()
    {
        _animationPlayer.Stop();
        _animationPlayer.Play("RESET");
        _label.Text = _texts[1];
    }

   
}
