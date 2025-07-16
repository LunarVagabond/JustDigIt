using Godot;
using System;

public partial class Oxygen : Pickup
{
	public override void _Ready()
	{
		base._Ready();
		GD.Print("Oxygen spawned");
	}
}
