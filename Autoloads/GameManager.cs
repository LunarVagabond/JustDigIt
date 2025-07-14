using Godot;
using IA = CustomInputActions.InputActions;

public partial class GameManager : Node
{
    // NOTE: Moved this out from player so ALL scenes get this interaction by default
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else if (@event.IsActionPressed(IA.TOGGLE_MOUSE_VISIBILITY))
        {
            if (Input.MouseMode == Input.MouseModeEnum.Visible)
            {
                Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }
    }
}
