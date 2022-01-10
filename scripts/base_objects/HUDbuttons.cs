using Godot;

public class HUDbuttons : NinePatchRect
{
    [Signal]
    delegate void PausePressed();
    [Signal]
    delegate void ResetPressed();
    [Signal]
    delegate void UndoPressed();
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
    public void _on_Undo_pressed()
    {
        EmitSignal(nameof(UndoPressed));
    }

}






