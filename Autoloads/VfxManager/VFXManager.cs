using Godot;
using System;

public partial class VFXManager : Node
{
	// public readonly PackedScene coin = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/coin.tscn");
	public readonly PackedScene poison = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/poison.tscn");
	public readonly PackedScene pickup = ResourceLoader.Load<PackedScene>("res://Entities//Interactables//GenericPickup//pickup.tscn");

	public void SpawnPickup(Vector2 spawnLocation, String pickupType)
	{
		Pickup newPickup = pickup.Instantiate() as Pickup;
		newPickup.Setup(spawnLocation, pickupType);
		AddChild(newPickup);
		GD.Print(newPickup, newPickup.GetType());
		// GD.Print("Pickup Entered Tree");
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
