using Godot;
using IA = CustomInputActions.InputActions;

public partial class Player : CharacterBody2D
{
	public const float Speed = 200.0f;
	public const float JumpVelocity = -200.0f;
	private SceneTransition sceneTransition;
	private AnimatedSprite2D playerSprite; // TODO: fix sprite or change, then FlipH

	public enum AnimState { Idle, Run, Climb, Death, Jump, }

	public AnimState CurrentState = AnimState.Idle;
	private UserInterface ui;
	private MiningRig miningRig;

	[Export]
	public CharacterStats stats;

	public override void _Ready()
	{
		// Scene Transition
		sceneTransition = GetNodeOrNull<SceneTransition>("/root/SceneTransition");
		sceneTransition.FadeIn();
		// Get Nodes
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		ui = GetNode<UserInterface>("UI");
		miningRig = GetNode<MiningRig>("MiningRig");
		// Set UI Display values
		GD.Print(GlobalPosition);
		ui.OxygenBar.Value = stats.oxygen;
		ui.LightBar.Value = stats.energy;
		ui.GoldCountLabel.Text = $"{stats.coins}";
		ui.DepthLevelLabel.Text = $"Depth: {GlobalPosition.Y}m";
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Loose Oxygen & Energy / Update Depth & Coins
		ui.OxygenBar.Value -= ui.OxygenBar.Step;
		ui.LightBar.Value -= ui.LightBar.Step;
		ui.GoldCountLabel.Text = $"{stats.coins}";
		ui.DepthLevelLabel.Text = $"Depth: {miningRig.level.LocalToMap(GlobalPosition).Y}m";

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed(IA.JUMP) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			SetAnimationState(AnimState.Jump); // FIXME: this isn't playing probably need to seperate logic into a state machine of sorts
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector(IA.MOVE_LEFT, IA.MOVE_RIGHT, IA.MOVE_UP, IA.MOVE_DOWN);
		if (direction != Vector2.Zero)
		{
			// FIXME: the architect in me says this is so bad, the jam timer says to bad so sad (we can look into optimization day 11)
			if (direction.X < 0) playerSprite.FlipH = true;
			if (direction.X > 0) playerSprite.FlipH = false;
			if (direction.X > 0 || direction.X < 0) SetAnimationState(AnimState.Run);
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			SetAnimationState(AnimState.Idle);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void SetAnimationState(AnimState state)
	{
		playerSprite.Play(state.ToString().ToLower());
	}
}
