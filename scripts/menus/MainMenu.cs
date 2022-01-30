using Godot;
using System;

public class MainMenu : MenuTemplates
{
    private AnimationPlayer _animationPlayer;

    [Signal]
    delegate void AnimationFinished();


    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");
    }

    public void StartAnimation(string name)
    {
        if (name == "Play")
        {
            _animationPlayer.Play("play");
        }
        else if (name == "Settings")
        {
             EmitSignal(nameof(AnimationFinished));
        }

    }
    public void _on_AnimationPlayer_animation_finished(string name)
    {
        if (name == "GoTo")
        {
            EmitSignal(nameof(AnimationFinished));
            return;
        }
         if (name == "play")
         {
            _animationPlayer.Play("GoTo");
         }

    }
}

