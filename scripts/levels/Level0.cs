using Godot;
using System;

public class Level0 : LevelWithTutorial
{
     private InputPausedAnimation _animationPlayer;

    [Export(PropertyHint.MultilineText)]
    protected String _text1;

    public override void _Ready()
    {
        base._Ready();

        _animationPlayer = GetNode<InputPausedAnimation>("AnimationPlayer");
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
