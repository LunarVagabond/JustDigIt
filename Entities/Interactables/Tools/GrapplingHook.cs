using Godot;
using System;
using IA = CustomInputActions.InputActions;

public partial class GrapplingHook : Node2D
{
	AnimationPlayer animationPlayer;
	private const string FIRE_ANIMATION = "Fire";
	private const string PICKUP_ANIMATION = "Pickup";
	private SceneTransition sceneTransition => GetNodeOrNull<SceneTransition>("/root/SceneTransition");
	private Area2D area2D;
	private Player player;
	private bool isDisplay = false;

	[Export(PropertyHint.File, "*.tscn")] private string NextLevel;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		player = GetParentOrNull<Player>();
		area2D = GetNodeOrNull<Area2D>("Area2D");
		Visible = false;
		animationPlayer.AnimationFinished += OnAnimationFinished;
		if (area2D is not null)
		{
			Visible = true;
			isDisplay = true;
			area2D.BodyEntered += OnBodyEntered;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(IA.FIRE_GRAPPLING_HOOK) && !isDisplay && player.foundgrapplingHook)
		{
			Visible = true;
			animationPlayer.Play(FIRE_ANIMATION);
		}
	}

	private void OnAnimationFinished(StringName name)
	{
		Visible = false;
		Player player = GetParent<Player>();
		player.gameManager.SavePlayer();
		player.gameManager.SaveLevel();
		player.gameManager.SaveBlueprints();
		player.gameManager.SaveEquippedTools();
		sceneTransition.ChangeScene(NextLevel);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			animationPlayer.Play(PICKUP_ANIMATION);
			player.foundgrapplingHook = true;
		}
	}
}
