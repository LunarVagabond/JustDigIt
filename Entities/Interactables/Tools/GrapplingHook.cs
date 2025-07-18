using Godot;
using System;
using IA = CustomInputActions.InputActions;

public partial class GrapplingHook : Node2D
{
	AnimationPlayer animationPlayer;
	private const string FIRE_ANIMATION = "Fire";
	private SceneTransition sceneTransition => GetNodeOrNull<SceneTransition>("/root/SceneTransition");

	[Export(PropertyHint.File, "*.tscn")] private string NextLevel;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.AnimationFinished += OnAnimationFinished;
		Visible = false;
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
		sceneTransition.ChangeScene(NextLevel);
	}

}
