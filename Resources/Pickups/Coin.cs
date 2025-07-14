using Godot;
using System;

public partial class Coin : Node2D
{
	public async void Setup(Vector2 targetLocation) // async
	{
		if (!IsInsideTree())
		{
			await ToSignal(this, "ready");
		}
		GlobalPosition = targetLocation;
		// GD.Print($"Coin Created: {targetLocation} -- {GlobalPosition}");
	}
}
