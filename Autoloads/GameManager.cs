using Godot;
using IA = CustomInputActions.InputActions;

public partial class GameManager : Node
{
    // Player player;

    // public override void _Ready()
    // {
    //     player = GetTree().GetFirstNodeInGroup("Player") as Player;
    // }

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

    // TODO: Handle different types here and add to the player stats or something in this
    public void HandlePickupCollected(Pickup.ItemType itemType, float value, Node2D body)
    {
        if (itemType == Pickup.ItemType.Coin && body is Player player)
        {
            GD.Print("Coin Gathered!");
            player.currentCoins += 1;
        }
    }
}
