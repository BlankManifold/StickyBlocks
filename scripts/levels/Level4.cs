using Godot;
using System;

public class Level4 : LevelWithTutorial
{
    private InputPausedAnimation _animationPlayer;

    [Export(PropertyHint.MultilineText)]
    protected String _text1;

    public override void _Ready()
    {
        base._Ready();

        _animationPlayer = (InputPausedAnimation)FindNode("AnimationPlayer");
        _animationPlayer.PlayBackAndForth = true;
        _animationPlayer.Play("tutorial");
        
        GetTree().Paused = true;
    }
    public void _on_AnimationPlayer_ActionPressed()
    {
        _animationPlayer.Stop();
        _animationPlayer.Play("RESET");
        _label.Text = _text1;
    }

   
}
