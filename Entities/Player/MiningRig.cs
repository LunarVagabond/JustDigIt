using Godot;
using System;
using System.Runtime.InteropServices;
using IA = CustomInputActions.InputActions;

public partial class MiningRig : Node2D
{
	private int miningRadius = 21;
	private int miningRadiusYOffset = -16;
	private float MouseSensitivity = 2.0f;

	// private RayCast2D rayCastRight;
	// private RayCast2D rayCastLeft;
	// private RayCast2D rayCastUp;
	// private RayCast2D rayCastDown;
	private MeshInstance2D miningTarget;
	// private Vector2 miningTargetPosition;
	private TileMapLayer level;
	private Player player;

	public override void _Ready()
	{
		base._Ready();

		// rayCastRight = GetNode<RayCast2D>("RayCastRight");
		// rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		// rayCastUp = GetNode<RayCast2D>("RayCastUp");
		// rayCastDown = GetNode<RayCast2D>("RayCastDown");
		miningTarget = GetNode<MeshInstance2D>("MiningTarget");
		level = GetNode<TileMapLayer>("/root/LevelOne/Level");
		player = GetParent<Player>();

		// _currentAngle = Mathf.Atan2(miningTarget.GlobalPosition.Y - GlobalPosition.Y, miningTarget.GlobalPosition.X - GlobalPosition.X);
		// GD.Print($"Mining Rig Located at: {GlobalPosition}, angle: {_currentAngle}");
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (Input.IsActionJustPressed(IA.MINE))
		{
			Vector2I tile = level.LocalToMap(miningTarget.GlobalPosition);
			// Vector2I tile = level.LocalToMap(miningTargetPosition);

			GD.Print("Mining action activated");
			GD.Print($"Position of mining target: {tile}");
			// int xAdjust = -3; // -2, -5
			// int yAdjust = -1; // -2, -3
			// Vector2I tile = LocalToMap(player.miningTargetPosition);
			// //tile.X -= 2;
			// //tile.Y -= 2;
			// tile.X += xAdjust; // how did these move?
			// tile.Y += yAdjust; // how did these move?
			// // GD.Print($"mine button used: {tile} -- {LocalToMap(player.rayCastRight.GlobalPosition)}");

			// if (crackedTiles.Contains(tile))
			// {
			// 	SetCell(2, tile); // deletes tile at layer 2 and pos
			// 	int index = crackedTiles.IndexOf(tile);
			// 	crackedTiles.RemoveAt(index);
			// 	tile.X -= xAdjust; // Something weird going on above, this give "correct" LocalMap() location
			// 	tile.Y -= yAdjust;
			// 	EmitSignal(SignalName.TileRemoved, tile);
			// }
			// else
			// {
			// 	// https://docs.godotengine.org/en/stable/classes/class_tilemap.html#class-tilemap-method-get-cell-source-id
			// 	// ideas -- https://www.youtube.com/watch?v=LNhFMaTYhZ8
			level.SetCell(tile, -1);
			// 	int sourceID = GetCellSourceId(2, tile);
			// 	if (sourceID != -1)
			// 	{
			// 		SetCell(2, tile, 0, new Vector2I(8, 0)); // Vector2I is atlas coordinates
			// 		crackedTiles.Add(tile);
			// 		GD.Print(crackedTiles.Count);
			// 	}
			// }
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			eventMouseMotion.ScreenVelocity *= MouseSensitivity;
			GD.Print(eventMouseMotion.ScreenVelocity);
			Vector2 mouseGlobalPosition = GetGlobalMousePosition();
			var playerPosition = player.GlobalPosition;
			// Get mouse relative to player
			var mouseDirection = (mouseGlobalPosition - playerPosition).Normalized();
			// Reposition mouse along circular radius relative to player
			mouseGlobalPosition = playerPosition + (mouseDirection * miningRadius);
			mouseGlobalPosition.Y += miningRadiusYOffset;
			miningTarget.GlobalPosition = mouseGlobalPosition;
			
			// RotateY(-mouseMotion.Relative.X * MouseSensitivity);
			// RotateX(-mouseMotion.Relative.Y * MouseSensitivity);

			// // Optional: Limit vertical rotation
			// Vector3 rotation = Rotation;
			// rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
			// Rotation = rotation;
		}
	}

	// Mining Target Controls if using Keyboard ----------------
	// Vector2 direction = Input.GetVector(IA.MOVE_LEFT, IA.MOVE_RIGHT, IA.MOVE_UP, IA.MOVE_DOWN);
	// if (direction != Vector2.Zero)
	// {
	// 	if (direction.X > 0)
	// 	{
	// 		miningTargetPosition = rayCastRight.GlobalPosition + rayCastRight.TargetPosition;
	// 		miningTarget.GlobalPosition = miningTargetPosition;
	// 	}
	// 	else if (direction.X < 0)
	// 	{
	// 		miningTargetPosition = rayCastLeft.GlobalPosition + rayCastLeft.TargetPosition;
	// 		miningTarget.GlobalPosition = miningTargetPosition;
	// 	}
	// 	if (direction.Y < 0)
	// 	{
	// 		miningTargetPosition = rayCastUp.GlobalPosition + rayCastUp.TargetPosition;
	// 		miningTarget.GlobalPosition = miningTargetPosition;
	// 	}
	// 	else if (direction.Y > 0)
	// 	{
	// 		miningTargetPosition = rayCastDown.GlobalPosition + rayCastDown.TargetPosition;
	// 		miningTarget.GlobalPosition = miningTargetPosition;
	// 	}
	// }
	// else
	// {
	// 	miningTarget.GlobalPosition = rayCastRight.GlobalPosition;
	// }
}
