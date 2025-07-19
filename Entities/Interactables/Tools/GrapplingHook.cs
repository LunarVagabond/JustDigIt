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

	[Export(PropertyHint.File, "*.tscn")] private string NextLevel;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		area2D = GetNodeOrNull<Area2D>("Area2D");
		Visible = false;
		animationPlayer.AnimationFinished += OnAnimationFinished;
		if (area2D is not null)
		{
			Visible = true;
			area2D.BodyEntered += OnBodyEntered;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed(IA.FIRE_GRAPPLING_HOOK))
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
		sceneTransition.ChangeScene(NextLevel);
	}

	private void OnBodyEntered(Node2D body)
	{
		// if (body is Player)
		// {
		// 	QueueFree();
		// }
		animationPlayer.Play(PICKUP_ANIMATION);
	}
}
