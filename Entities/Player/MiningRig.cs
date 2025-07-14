using Godot;
using System;
using System.Runtime.InteropServices;
using IA = CustomInputActions.InputActions;

public partial class MiningRig : Node2D
{
	private int miningRadius = 21;
	private int miningRadiusYOffset = -16;
	private float MouseSensitivity = 2.0f;
	private MeshInstance2D miningTarget;
	private TileMapLayer level;
	private Player player;
	private Godot.Collections.Array<Vector2> crackedTiles = [];
	private AudioManager audioManager;

	private AudioStream miningSFX = GD.Load<AudioStream>("res://Assets/SFX/weapon-axe-hit-01-153372.mp3");

	public override void _Ready()
	{
		base._Ready();

		miningTarget = GetNode<MeshInstance2D>("MiningTarget");
		level = GetNode<TileMapLayer>("/root/LevelOne/Level");
		audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
		player = GetParent<Player>();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (Input.IsActionJustPressed(IA.MINE))
		{
			Vector2I tile = level.LocalToMap(miningTarget.GlobalPosition);

			// GD.Print("Mining action activated");
			// GD.Print($"Position of mining target: {tile}");

			if (crackedTiles.Contains(tile))
			{
				audioManager.PlaySfx(miningSFX);
				level.SetCell(tile, -1); // deletes tile at layer 2 and pos
				int index = crackedTiles.IndexOf(tile);
				crackedTiles.RemoveAt(index);
				// EmitSignal(SignalName.TileRemoved, tile);
			}
			else
			{
				// https://docs.godotengine.org/en/stable/classes/class_tilemap.html#class-tilemap-method-get-cell-source-id
				// ideas -- https://www.youtube.com/watch?v=LNhFMaTYhZ8
				int sourceID = level.GetCellSourceId(tile);
				if (sourceID == 0)
				{
					level.SetCell(tile, 1, new Vector2I(5, 0)); // Vector2I is atlas coordinates
					crackedTiles.Add(tile);
					audioManager.PlaySfx(miningSFX);
					GD.Print(crackedTiles.Count);
				}
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Moves the dig locator around the player (?)
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			eventMouseMotion.ScreenVelocity *= MouseSensitivity;
			Vector2 mouseGlobalPosition = GetGlobalMousePosition();
			var playerPosition = player.GlobalPosition;
			// Get mouse relative to player
			var mouseDirection = (mouseGlobalPosition - playerPosition).Normalized();
			// Reposition mouse along circular radius relative to player
			mouseGlobalPosition = playerPosition + (mouseDirection * miningRadius);
			mouseGlobalPosition.Y += miningRadiusYOffset;
			miningTarget.GlobalPosition = mouseGlobalPosition;
		}
	}
}
