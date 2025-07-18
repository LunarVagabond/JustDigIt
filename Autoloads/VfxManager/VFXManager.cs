using Godot;
using Godot.Collections;
using System;

public partial class VFXManager : Node
{
	public readonly PackedScene poison = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/poison.tscn");
	public readonly PackedScene pickup = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/GenericPickup/pickup.tscn");
	public readonly PackedScene coin = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/Coin//coin.tscn");
	public readonly PackedScene oxygen = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/Oxygen/oxygen.tscn");
	public readonly PackedScene key = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/hidden_room_key.tscn");

	public void SpawnPickup(Vector2 spawnLocation, Pickup.ItemType pickupType)
	{
		Pickup newPickup;
		switch (pickupType)
		{
			case Pickup.ItemType.Coin:
				newPickup = coin.Instantiate<Pickup>();
				break;
			case Pickup.ItemType.Oxygen:
				newPickup = oxygen.Instantiate<Pickup>();
				break;
			case Pickup.ItemType.Battery:
				newPickup = oxygen.Instantiate<Pickup>(); // FIXME: Battery DNE, so falling back for now to oxygen
				break;
			default:
				GD.PrintErr($"Incorrect ItemType came through for spawn: {pickupType}");
				newPickup = oxygen.Instantiate<Pickup>();
				break;
		}
		newPickup.GlobalPosition = spawnLocation;
		AddChild(newPickup);
	}

	public void SpawnPoison(Vector2 targetLocation)
	{
		Poison newPoison = poison.Instantiate() as Poison;
		newPoison.Setup(targetLocation);
		AddChild(newPoison);
	}

	public void SpawnKey(Vector2 targetLocation)
	{
		HiddenRoomKey newKey = key.Instantiate<HiddenRoomKey>();
		newKey.Setup(targetLocation);
		AddChild(newKey);
	}
}
