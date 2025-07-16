using Godot;
using Godot.Collections;
using System;

public partial class VFXManager : Node
{
	public readonly PackedScene poison = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/poison.tscn");
	public readonly PackedScene pickup = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/GenericPickup/pickup.tscn");
	public readonly PackedScene coin = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/Coin//coin.tscn");
	public readonly PackedScene oxygen = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/Oxygen/oxygen.tscn");

	public void SpawnPickup(Vector2 spawnLocation, Pickup.ItemType pickupType)
	{
		Pickup newPickup;
		// PickupRes pickupResource = ResourceLoader.Load<PickupRes>(pickupResPaths[pickupType]);

		switch (pickupType)
		{
			case Pickup.ItemType.Coin:
				newPickup = coin.Instantiate<Pickup>();
				break;
			case Pickup.ItemType.Oxygen:
				newPickup = oxygen.Instantiate<Pickup>();
				break;
			// FIXME: This scene doesn't exist yet
			// case Pickup.ItemType.Battery:
			//   newPickup = battery.Instantiate<Pickup>();
			// 	break;
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
}
