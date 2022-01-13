using Godot;

public class InputPausedAnimation : AnimationPlayer
{
    [Export]
    private string _action = "nextText";

    [Export]
    private string _name = "tutorial";

    private bool _animationBackwards = false;
    private bool _playBackAndForth = false;
    public bool PlayBackAndForth { get { return _playBackAndForth; } set { _playBackAndForth = value; } }

    [Signal]
    delegate void ActionPressed();


    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (GetTree().Paused)
        {
            if (inputEvent.IsActionPressed(_action))
            {
                GetTree().Paused = false;
                EmitSignal(nameof(ActionPressed));
            }

        }
        inputEvent.Dispose();
    }


    public void _on_AnimationPlayer_animation_finished(string name)
    {
        if (_playBackAndForth)
        {

            if (name == _name)
            {
                if (_animationBackwards)
                {
                    Play(_name);
                }
                else
                {
                    PlayBackwards(_name);
                }

                _animationBackwards = !_animationBackwards;
            }
        }
    }
}
