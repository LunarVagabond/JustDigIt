using Godot;
using System;
using IA = CustomInputActions.InputActions;

public partial class ElectricLock : Area2D
{
	private Label InteractButtonLabel;
	private Player player;
	private TileMapLayer hiddenRoom;
	private TileMapLayer hiddenRoomCovering;

	private bool isOverlapping = false; // FIXME: this is a messy way of doing this but hey it works for now

	public override void _Ready()
	{
		hiddenRoom = GetNodeOrNull<TileMapLayer>("/root/LevelOne/HiddenRoom"); // FIXME: dynamically select level
		hiddenRoomCovering = GetNodeOrNull<TileMapLayer>("/root/LevelOne/HiddenRoomCovering");
		InteractButtonLabel = GetNode<Label>("InteractionLabel");
		InteractButtonLabel.Visible = false;
		BodyEntered += OnBodyEntered;
		Visible = false;
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(IA.INTERACT) && isOverlapping)
		{
			if (player.levelKey)
			{
				OpenLock();
			}
			else
			{
				GD.Print("Door still locked");
			}
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		GD.Print(body, body is Player);
		if (body is Player)
		{
			player = body as Player;
			InteractButtonLabel.Visible = true;
			isOverlapping = true;
			Visible = true;
			// GD.Print(player, InteractButtonLabel.Visible, isOverlapping);
		}
	}
	private void OnBodyExited(Node2D body)
	{
		InteractButtonLabel.Visible = false;
		isOverlapping = false;
	}

	private void OpenLock()
	{
		GD.Print("Lock opened!");

		// For now, manually designated doors to open
		hiddenRoom.SetCell(new Vector2I(9, 12), 4, new Vector2I(4, 10), 1);
		hiddenRoom.SetCell(new Vector2I(9, 13), 4, new Vector2I(4, 11), 1);
		// background.SetCell(new Vector2I(9, 12), -1);
		// background.SetCell(new Vector2I(9, 13), -1);
		hiddenRoomCovering.QueueFree(); // Make Hidden Room fully visible
	}
}
