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
        _animationPlayer = (AnimationPlayer)FindNode("AnimationPlayer");
        TextureRect block = (TextureRect)FindNode("Block");
        block.RectPosition = new Vector2(467, 327);
    }

    public void StartAnimation(string name)
    {
        if (name == "Play")
        {
            _animationPlayer.Play("PlaySelected");

        }
        else if (name == "Options")
        {
            _animationPlayer.Play("OptionsSelected");
        }

    }
    public void _on_AnimationPlayer_animation_finished(string name)
    {
        if (name == "GoTo")
        {
            EmitSignal(nameof(AnimationFinished));
            return;
        }
        _animationPlayer.Play("GoTo");

    }
}



