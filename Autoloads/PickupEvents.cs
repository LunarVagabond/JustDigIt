using Godot;

public partial class PickupEvents : Node
{
  [Signal] public delegate void PickupCollectedEventHandler(Pickup pickup); // Collector will always be player this might be redundant

  public void EmitPickupCollected(Pickup pickup) => EmitSignal(SignalName.PickupCollected, pickup);

  // TODO: Handle different types here and add to the player stats or something in this
}
