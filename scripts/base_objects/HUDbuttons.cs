using Godot;

public class HUDbuttons : NinePatchRect
{
    [Signal]
    delegate void PausePressed();
    [Signal]
    delegate void ResetPressed();
    public override void _Ready()
    {

    }
    public void _on_Pause_pressed()
    {
        EmitSignal(nameof(PausePressed));
    }
    public void _on_Reset_button_down()
    {
        EmitSignal(nameof(ResetPressed));
    }

}







