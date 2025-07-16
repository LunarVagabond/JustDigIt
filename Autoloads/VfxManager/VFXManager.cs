using Godot;
using System;

public partial class VFXManager : Node
{
	public readonly PackedScene poison = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/poison.tscn");
	public readonly PackedScene pickup = ResourceLoader.Load<PackedScene>("res://Entities//Interactables//GenericPickup//pickup.tscn");
	public readonly PackedScene coin = ResourceLoader.Load<PackedScene>("res://Entities//Interactables//Coin//coin.tscn");
	public readonly PackedScene oxygen = ResourceLoader.Load<PackedScene>("res://Entities//Interactables//Oxygen//oxygen.tscn");

	public void SpawnPickup(Vector2 spawnLocation, String pickupType)
	{
		// This is messy, but I could not find a cleaner way to make it work.
		Pickup newPickup;
		if (pickupType == "coin")
		{
			newPickup = coin.Instantiate() as Coin;
			newPickup = (Coin)newPickup;
		}
		else if (pickupType == "oxygen")
		{
			newPickup = oxygen.Instantiate() as Oxygen;
			newPickup = (Oxygen)newPickup;
		}
		else
		{
			newPickup = pickup.Instantiate() as Pickup;
		}
		newPickup.Setup(spawnLocation, pickupType);
		AddChild(newPickup);
	}

	// public void SpawnCoin(Vector2 targetLocation)
	// {
	// 	Coin newCoin = coin.Instantiate() as Coin;
	// 	newCoin.Setup(targetLocation);
	// 	AddChild(newCoin);
	// }
	
	public void SpawnPoison(Vector2 targetLocation)
	{
		Poison newPoison = poison.Instantiate() as Poison;
		newPoison.Setup(targetLocation);
		AddChild(newPoison);
	}
}
