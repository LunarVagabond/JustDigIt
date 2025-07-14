using Godot;

public partial class PickupEvents : Node
{
  [Signal] public delegate void PickupCollectedEventHandler(Pickup pickup, Node collector); // Collector will always be player this might be redundant

  public void EmitPickupCollected(Pickup pickup, Node collector) => EmitSignal(SignalName.PickupCollected, pickup, collector);

  // TODO: Handle different types here and add to the player stats or something in this
}
