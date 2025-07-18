using Godot;
using System;
using System.Runtime.InteropServices;
using IA = CustomInputActions.InputActions;

public partial class MiningRig : Node2D
{
	[Signal]
	public delegate void TileRemovedEventHandler(Vector2 rigLocation, float coinProbability, int sourceID);

	private int miningRadius = 21;
	private int miningRadiusYOffset = -16;
	private int miningRadiusXOffset = 1;
	private float MouseSensitivity = 2.0f;
	private MeshInstance2D miningTarget;
	public TileMapLayer level;
	private Player player;
	private Godot.Collections.Array<Vector2> crackedTiles = [];
	private Godot.Collections.Array<float> coinProbability = [];
	private AudioManager audioManager;
	private AudioStream miningSFX = GD.Load<AudioStream>("res://Assets/SFX/weapon-axe-hit-01-153372.mp3");
	// public readonly PackedScene key = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/hidden_room_key.tscn");
	public VFXManager vfxManager;
	private float hazardProbability = 0.8f;

	public override void _Ready()
	{
		base._Ready();

		miningTarget = GetNode<MeshInstance2D>("MiningTarget");
		level = GetNodeOrNull<TileMapLayer>("/root/LevelOne/Level"); // FIXME: dynamically select level
		audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
		vfxManager = GetNodeOrNull<VFXManager>("/root/VfxManager");
		player = GetParent<Player>();
		TileRemoved += HandleTileRemoved;

		// Prob need a more robust way to check this, along with dynamically loading level
		if (level is null)
		{
			SetPhysicsProcess(false);
			SetProcessInput(false);
			miningTarget.Visible = false;
			player.MiningRigEnabled = false;
		}
		else
		{
			player.MiningRigEnabled = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (Input.IsActionJustPressed(IA.MINE))
		{
			Vector2I tile = level.LocalToMap(miningTarget.GlobalPosition);

			// GD.Print("Mining action activated");
			GD.Print($"Position of mining target: {tile}");

			if (crackedTiles.Contains(tile))
			{
				audioManager.PlaySfx(miningSFX);
				level.SetCell(tile, -1); // deletes tile at layer 2 and pos
				int index = crackedTiles.IndexOf(tile);
				EmitSignal(SignalName.TileRemoved, miningTarget.GlobalPosition, coinProbability[index], 0); // hard coded last variable
				crackedTiles.RemoveAt(index);
				coinProbability.RemoveAt(index);
			}
			else
			{
				// https://docs.godotengine.org/en/stable/classes/class_tilemap.html#class-tilemap-method-get-cell-source-id
				// ideas -- https://www.youtube.com/watch?v=LNhFMaTYhZ8
				int sourceID = level.GetCellSourceId(tile);
				Vector2I atlasCoord = level.GetCellAtlasCoords(tile);
				float probability = GetResourceProbability(atlasCoord);

				if (sourceID == 0)
				{
					level.SetCell(tile, 1, new Vector2I(5, 0)); // Vector2I is atlas coordinates
					crackedTiles.Add(tile);
					coinProbability.Add(probability);
					audioManager.PlaySfx(miningSFX);
					// GD.Print($"{crackedTiles.Count} -- {probability} -- {atlasCoord}");
				}
				else if (sourceID == 1)
				{
					audioManager.PlaySfx(miningSFX);
					level.SetCell(tile, -1);
					EmitSignal(SignalName.TileRemoved, miningTarget.GlobalPosition, 0.0, 1);
				}
			}
		}
	}

	private float GetResourceProbability(Vector2I tile)
	{
		float probability = tile switch
		{
			// TODO: These float values should prob be an enum
			Vector2I(1, 3) => 0.0f, // for hidden room edge
			Vector2I(7, 16) => 0.1f,
			Vector2I(13, 16) => 0.15f,
			Vector2I(21, 16) => 0.2f,
			Vector2I(29, 16) => 0.25f,
			Vector2I(30, 11) => 0.5f,
			_ => 0.05f
		};

		return probability;
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
			mouseGlobalPosition.X += miningRadiusXOffset;
			miningTarget.GlobalPosition = mouseGlobalPosition;
		}
	}


	private void HandleTileRemoved(Vector2 targetLocation, float probability, int sourceID)
	{
		// GD.Print($"{targetLocation} -- {probability}");
		Random rnd = new Random();
		float roll = (float)rnd.NextDouble();
		if (sourceID == 1) // for key generation
		{
			GD.Print("Hidden Room key revealed!");
			vfxManager.SpawnKey(targetLocation);
			// Key hiddenRoomKey = key.Instantiate<Key>();
			// newPickup.GlobalPosition = spawnLocation;
			// AddChild(newPickup);
		}
		else if (roll < probability)
		{
			// vfxManager.SpawnCoin(targetLocation);
			vfxManager.SpawnPickup(targetLocation, Pickup.ItemType.Coin); // Fix loose string, make enum

			// GD.Print("Coin Spawned");
		}
		else if (roll > hazardProbability)
		{
			vfxManager.SpawnPoison(targetLocation); // Probably can make a hazard resource like pickup...
		}
	}
}
