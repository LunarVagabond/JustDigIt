using Godot;
using System;

public partial class Poison : Area2D
{
	Player player;

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		BodyEntered += HandleBodyEntered;
	}

	public async void Setup(Vector2 targetLocation)
	{
		if (!IsInsideTree())
		{
			await ToSignal(this, "ready");
		}
		GlobalPosition = targetLocation;
		// GD.Print($"Poison Created: {targetLocation} -- {GlobalPosition}");
	}

	public void HandleBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.stats.poisoned = true;
			player.poisonTimer.Start(player.stats.poisonDuration);
			GD.Print("Poisoned!");
		}
	}
}
