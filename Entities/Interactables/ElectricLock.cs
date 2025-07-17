using Godot;
using System;
using IA = CustomInputActions.InputActions;

public partial class ElectricLock : Area2D
{
	private Label InteractButtonLabel;
	private Player player;
	private TileMapLayer background;

	private bool isOverlapping = false; // FIXME: this is a messy way of doing this but hey it works for now

	public override void _Ready()
	{
		background = GetNodeOrNull<TileMapLayer>("/root/LevelOne/Background"); // FIXME: dynamically select level
		InteractButtonLabel = GetNode<Label>("InteractionLabel");
		InteractButtonLabel.Visible = false;
		BodyEntered += OnBodyEntered;
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(IA.INTERACT) && isOverlapping)
		{
			OpenLock();
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
			GD.Print(player, InteractButtonLabel.Visible, isOverlapping);
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
		background.SetCell(new Vector2I(9, 12), 4, new Vector2I(4, 10), 1);
		background.SetCell(new Vector2I(9, 13), 4, new Vector2I(4, 11), 1);
		// background.SetCell(new Vector2I(9, 12), -1);
		// background.SetCell(new Vector2I(9, 13), -1);
	}
}
