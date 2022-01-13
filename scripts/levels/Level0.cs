using Godot;
using System;

public class Level0 : Level
{
    private String[] _texts = new String[] { "OBJECTIVE:  the red block must reach the last Stickyblock marked with the small red block, complete the level in as few moves as possible to owned it. \n\n Press SPACEBAR to continue", 
                                            "Press SPACEBAR to dash along the perpendicular direction" };
    private Label _label;
    private AnimationPlayer _animationPlayer;
    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];

        _animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");
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
