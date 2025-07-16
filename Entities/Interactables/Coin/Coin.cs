using Godot;
using System;

public partial class Coin : Pickup
{
	public override void _Ready()
	{
		base._Ready();
		GD.Print("I am a coin");
	}
}
