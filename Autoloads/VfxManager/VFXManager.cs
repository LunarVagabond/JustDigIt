using Godot;
using System;

public partial class VFXManager : Node
{
	public readonly PackedScene coin = ResourceLoader.Load<PackedScene>("res://Resources/coin.tscn");
	private float coinOffsetY = 1.0f;
	private float coinOffsetX = 0.5f;
	
	public void SpawnCoin(Vector2 targetLocation)
	{
		// targetLocation = targetLocation with
		// {
		// 	Y = targetLocation.Y + coinOffsetY,
		// 	X = targetLocation.X + coinOffsetX
		// };
		Coin newCoin = coin.Instantiate() as Coin;
		newCoin.Setup(targetLocation);
		AddChild(newCoin);
	}
}
