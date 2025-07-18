using Godot;
using System;

public partial class HiddenRoomKey : Area2D
{
	private Player player;
	protected AnimationPlayer animationPlayer;
	// protected GameManager gameManager;
	private const string PICKUP_ANIMATION = "Pickup";

	public override void _Ready()
	{
		player = GetTree().GetFirstNodeInGroup("Player") as Player;
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		BodyEntered += HandleBodyEntered;
	}

	public async void Setup(Vector2 targetLocation)
	{
		if (!IsInsideTree())
		{
			await ToSignal(this, "ready");
		}
		GlobalPosition = targetLocation;
		GD.Print($"Key Created: {targetLocation} -- {GlobalPosition}");
	}

	public void HandleBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.levelKey = true;
			animationPlayer.Play(PICKUP_ANIMATION);      
			GD.Print("Key Collected!");
		}
	}
}
