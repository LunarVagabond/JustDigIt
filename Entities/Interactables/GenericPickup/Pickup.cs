using Godot;

public partial class Pickup : Area2D
{
	[Export] public PickupRes Item;
	[Export] public int Worth;

	private TextureRect iconNode;
	protected PickupEvents pickupEventsGlobal;

	public enum ItemType
	{
		Oxygen,
		Battery,
		Coin
	}

	public override void _Ready()
	{
		iconNode = GetNode<TextureRect>("Control/ItemIcon");
		pickupEventsGlobal = GetNode<PickupEvents>("/root/PickupEvents");
		iconNode.Texture = Item.icon;
	}


	// Based on collision settings this will and should only ever be the player
	public virtual void OnItemBodyEnter(Node2D body)
	{
		pickupEventsGlobal.EmitSignal(PickupEvents.SignalName.PickupCollected, this, body);
		QueueFree();
	}
}
