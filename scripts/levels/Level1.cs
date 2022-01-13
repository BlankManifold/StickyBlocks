using Godot;
using System;

public class Level1 : Level
{
    private String[] _texts = new String[] {"Different colors represent different rotation angles. \n\n Press SPACEBAR to continue", 
                                            "LEFT CLICK on the StickyBlock to rotate it back and forth" };
    private Label _label;
    private InputPausedAnimation _animationPlayer;

    private Vector2 _position;
    public override void _Ready()
    {
        base._Ready();

        CanvasLayer layer = (CanvasLayer)FindNode("HUDLayer");
        _label = layer.GetNode<Label>("Label");
        _label.Text = _texts[0];

        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");

        Vector2 _position = _playerBlock.GlobalPosition;
        RemoveChild(_playerBlock);
        initialBlock.AddChild(_playerBlock);
        _playerBlock.GlobalPosition = _position;

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

        RotationStickyBlock initialBlock = (RotationStickyBlock)FindNode("InitialBlock");
        initialBlock.RemoveChild(_playerBlock);
        AddChild(_playerBlock);
        ResetLevel();
    }

   
}
