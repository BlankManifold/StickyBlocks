using Godot;
using System;

public class Level1 : LevelWithTutorial
{
    private InputPausedAnimation _animationPlayer;

    private Vector2 _position;

    [Export(PropertyHint.MultilineText)]
    protected String _text1;
    public override void _Ready()
    {
        base._Ready();

        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");

        Vector2 _position = _playerBlock.GlobalPosition;
        RemoveChild(_playerBlock);
        initialBlock.AddChild(_playerBlock);
        _playerBlock.GlobalPosition = _position;

        _animationPlayer = GetNode<InputPausedAnimation>("AnimationPlayer");
        _animationPlayer.PlayBackAndForth = true;
        _animationPlayer.Play("tutorial");
        
        GetTree().Paused = true;
    }
    public void _on_AnimationPlayer_ActionPressed()
    {
        _animationPlayer.Stop();
        _animationPlayer.Play("RESET");
        _label.Text = _text1;

        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");
        initialBlock.RemoveChild(_playerBlock);
        AddChild(_playerBlock);
        ResetLevel();
    }

   
}
