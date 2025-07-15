using Godot;

public partial class PickupEvents : Node
{
    [Signal] public delegate void PickupCollectedEventHandler(Pickup pickup); // Collector will always be player this might be redundant
    
    public void EmitPickupCollected(Pickup pickup) => EmitSignal(SignalName.PickupCollected, pickup); // What is this doing?

    // TODO: Handle different types here and add to the player stats or something in this
    public void HandlePickupCollected(Pickup pickup)
    {
        GD.Print("Got to handle pickup global spot");
        if (pickup.Item.itemType == Pickup.ItemType.Coin)
            {
                GD.Print("Coin Gathered!");
            }
    }
}
