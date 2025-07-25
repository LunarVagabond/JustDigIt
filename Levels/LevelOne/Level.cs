using Godot;
using System;

public partial class Level : TileMapLayer
{
	private bool loaded = false;
	private GameManager gameManager;
	public Godot.Collections.Array<Vector2I> emptyCells;
	public Godot.Collections.Array<int> emptyCellsX;
	public Godot.Collections.Array<int> emptyCellsY;

	[Export] Player player;

	[Signal] public delegate void LevelLoadedEventHandler();

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		Ready += LoadLevel;
		GD.Print("Map on Creation");
		GetEmptyCellPositionsInRect();
	}

	public Godot.Collections.Dictionary<string, Variant> Save()
	{
		return new Godot.Collections.Dictionary<string, Variant>()
		{
			{ "Filename", "res://Levels/LevelOne/level_one.tscn" },
			{ "Parent", GetParent().GetPath() },
			{ "CollisionEnabled", CollisionEnabled },
			{ "Enabled", Enabled},
			{ "NavigationEnabled", NavigationEnabled},
			{ "OcclusionEnabled", OcclusionEnabled },
			{ "RenderingQuadrantSize", RenderingQuadrantSize },
			{ "TileMapData", TileMapData },
			{ "TileSet", TileSet},
			{ "UseKinematicBodies", UseKinematicBodies },
			{ "XDrawOrderReversed", XDrawOrderReversed },
			{ "YSortOrigin", YSortOrigin },
			{ "emptyCellsX", emptyCellsX },
			{ "emptyCellsY", emptyCellsY }
		};
	}

	private void LoadLevel() // async
	{
		if (loaded == false)
		{
			// if (!player.IsInsideTree())
			// {
			// 	await ToSignal(player, "ready");
			// }
			// if (!player.gameManager.IsInsideTree())
			// {
			// 	await ToSignal(player.gameManager, "ready");
			// }

			Node2D level = GetNodeOrNull<Node2D>("/root/LevelOne");
			if (level is not null && FileAccess.FileExists($"user://levels/{level.Name}.save"))
			{
				GD.Print("=====================");
				GD.Print(level, level.Name);
				// GD.Print(player);
				GD.Print(gameManager);
				gameManager.LoadLevel(level, "LevelOne"); // hard code for now?
				GD.Print("Attempting to load Map...");
				GD.Print("=====================");
				loaded = true;
			}
			else
			{
				GD.Print(level);
			}
		}
		EmitSignal(SignalName.LevelLoaded);
	}

	public void GetEmptyCellPositionsInRect()
	{
		int emptyCount = 0;
		int totalCount = 0;
		Rect2 rect = GetUsedRect();
		emptyCells = [];
		emptyCellsX = [];
		emptyCellsY = [];

		for (int i = -(int) rect.Size.X +1; i < rect.Size.X - 1; i += 1)
		{
			for (int k = -1; k < rect.Size.Y - 1; k += 1)
			{
				int cellID = GetCellSourceId(new Vector2I(i, k));
				if (cellID == -1)
				{
					// GD.Print($"({i}, {k})");
					emptyCount += 1;
					emptyCells.Add(new Vector2I(i, k));
					emptyCellsX.Add(i);
					emptyCellsY.Add(k);
				}
				totalCount += 1;
			}
		}
		GD.Print($"{emptyCount} -- {emptyCells.Count} -- {emptyCellsX.Count} -- {emptyCellsY.Count}");
		GD.Print(totalCount);
	}	
}
