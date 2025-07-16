using Godot;
using System;

public partial class Coin : Pickup
{
	public override void _Ready()
	{
		base._Ready();
		GD.Print("Coin Spawned");
		// GD.Print("I am a coin");
	}
}
