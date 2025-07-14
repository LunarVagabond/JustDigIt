using Godot;

public partial class Pickup : Area2D
{

	// TODO: We could use a resource here of Pickup -> Clone the resource in each plament in the level -> never need to set icon or anything again just let resoure handle this
	[Export] public ItemType itemType;
	[Export] int Worth;
	[Export] Texture2D Icon;

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
		iconNode.Texture = Icon;
	}


	// Based on collision settings this will and should only ever be the player
	public virtual void OnItemBodyEnter(Node2D body)
	{
		GD.Print("Body Entered");
		pickupEventsGlobal.EmitSignal(PickupEvents.SignalName.PickupCollected, this, body);
		QueueFree();
	}
}
