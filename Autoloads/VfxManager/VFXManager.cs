using Godot;
using System;

public partial class VFXManager : Node
{
	public readonly PackedScene coin = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/coin.tscn");
	public readonly PackedScene poison = ResourceLoader.Load<PackedScene>("res://Resources/Pickups/poison.tscn");

	public void SpawnCoin(Vector2 targetLocation)
	{
		Coin newCoin = coin.Instantiate() as Coin;
		newCoin.Setup(targetLocation);
		AddChild(newCoin);
	}
	
	public void SpawnPoison(Vector2 targetLocation)
	{
		Poison newPoison = poison.Instantiate() as Poison;
		newPoison.Setup(targetLocation);
		AddChild(newPoison);
	}
}
