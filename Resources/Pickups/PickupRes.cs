using Godot;

[GlobalClass]
public partial class PickupRes : Resource
{
  [Export] public Texture2D icon;
  [Export] public Pickup.ItemType itemType;
}