using Godot;

public class OptionsMenu : MenuTemplates
{
    private AnimationPlayer _animationPlayer;
    private string _buttonPressed = "Return";

    [Signal]
    delegate void AnimationFinished();
    [Signal]
    delegate void ReturnPressed();
    public override void _Ready()
    {
        base._Ready();
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        Connect("ReturnPressed", GetTree().Root.GetChild(0), "_on_OptionsMenu_Return_Pressed");
    }

    public void _on_AnimationPlayer_animation_finished(string name)
    {
        if (name == "resetData")
        {
            EmitSignal(nameof(AnimationFinished));
        }
    }

    public void _on_Return_pressed()
    {
        EmitSignal(nameof(ReturnPressed));
    }

    public async void _on_Reset_pressed()
    {
        _gameManager.LoadDefaultPlayerData();
        _gameManager.SavePlayerData();
        
        _animationPlayer.Play("resetData");
        await ToSignal(this,nameof(AnimationFinished));
    }
}



