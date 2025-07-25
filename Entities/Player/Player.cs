using System;
using System.Transactions;
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
	public UserInterface ui;
	public MiningRig miningRig;
	public bool poisoned = false;
	public float OxygenLossRate = 1.0f;
	float drain;
	public float PoisonEffect;
	private int depthOffset = 1;
	private int currentDepth = 1;
	public int currentCoins;
	public float currentOxygen;
	public float currentEnergy;
	public bool levelKey = false; // prob needs an array in stats for all level keys?
	public bool roomOpened = false;
	public bool beenToLevelOne = false;
	public bool foundgrapplingHook = false;

	public bool MiningRigEnabled = false;

	public GameManager gameManager;
	public bool loaded = false;
	public TileMapLayer hiddenRoomCovering;
	public TileMapLayer hiddenRoom;
	public GrapplingHook displayGrapplingHook;
	public Poison displayPoison;
	public ElectricLock electricLock;
	public Node2D industrialWaste;
	public Node2D hiddenRoomNote;
	public String equippedItem;
	public int currentMiningSkill;


	[Export]
	public Timer poisonTimer;
	[Export]
	public CharacterStats stats;

	[Signal] public delegate void PlayerLoadedEventHandler();

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		// Scene Transition
		sceneTransition = GetNodeOrNull<SceneTransition>("/root/SceneTransition");
		sceneTransition.FadeIn();
		// Get Nodes
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		ui = GetNode<UserInterface>("UI");
		miningRig = GetNode<MiningRig>("MiningRig");
		hiddenRoomCovering = GetNodeOrNull<TileMapLayer>("/root/LevelOne/HiddenRoomCovering");
		hiddenRoom = GetNodeOrNull<TileMapLayer>("/root/LevelOne/HiddenRoom");
		displayGrapplingHook = GetNodeOrNull<GrapplingHook>("/root/LevelOne/GrapplingHook");
		displayPoison = GetNodeOrNull<Poison>("/root/LevelOne/Poison");
		electricLock = GetNodeOrNull<ElectricLock>("/root/LevelOne/ElectricLock");
		industrialWaste = GetNodeOrNull<Node2D>("/root/LevelOne/IndustrialWaste");
		hiddenRoomNote = GetNodeOrNull<Node2D>("/root/LevelOne/HiddenRoom1Note");

		if (industrialWaste is not null) industrialWaste.Visible = false;
		if(hiddenRoomNote is not null ) hiddenRoomNote.Visible = false;

		// Set current stat values
		currentMiningSkill = stats.miningSkill;
		currentDepth = stats.depth;
		// currentCoins = stats.coins; // unneeded, set by LoadPlayer
		currentOxygen = stats.maxOxygen;
		currentEnergy = stats.maxEnergy;
		PoisonEffect = stats.PoisonEffect;
		drain = OxygenLossRate;
		ui.darknessEffect.UpdateDarknessLarge(GlobalPosition);
		// Signals
		poisonTimer.Timeout += HandlePoisonTimeout;

		gameManager.player = this; // Set player in game manger once the player dictates it's ready
		gameManager.ui = ui;

		Ready += LoadPlayer;
		// RebuildRecipes();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (MiningRigEnabled) HandleStats();
		ui.GoldCountLabel.Text = $"{currentCoins}"; // Do this on homestead, for crafting costs
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
			SetAnimationState(AnimState.Jump); // FIXME: this isn't playing probably need to seperate logic into a state machine of sorts
		}

		// Handle Jump.
		if (Input.IsActionJustPressed(IA.JUMP) && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
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
			if (IsOnFloor())
			{
				SetAnimationState(AnimState.Idle);
			}
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void SetAnimationState(AnimState state)
	{
		playerSprite.Play(state.ToString().ToLower());
	}

	private void HandleStats()
	{
		// Loose Oxygen & Energy / Update Depth & Coins
		// currentCoins handled bu PickupCollected event in GameManager
		// if (poisoned) currentOxygen -= (float)ui.OxygenBar.Step * PoisonEffect;
		OxygenLossRate = MathF.Round(currentDepth / 10.0f, 1); // Can play around with this
																													 // GD.Print($"Current rate of oxygen loss {OxygenLossRate}");
		if (poisoned) drain = OxygenLossRate * PoisonEffect; // This is prob overcomplicated, but my brain hurts
		currentOxygen -= (float)ui.OxygenBar.Step * drain;
		currentDepth = miningRig.level.LocalToMap(GlobalPosition).Y + depthOffset;
		// Handles both the UI and the energy stat directly
		ui.darknessEffect.HandleEnergyDrain(ui.LightBar, this, GlobalPosition);
		// Update UI
		ui.OxygenBar.Value = currentOxygen;
		ui.DepthLevelLabel.Text = $"Depth: {currentDepth}m";
		// ui.GoldCountLabel.Text = $"{currentCoins}";
	}

	public Godot.Collections.Dictionary<string, Variant> Save()
	{
		return new Godot.Collections.Dictionary<string, Variant>()
		{
			{ "Filename", SceneFilePath },
			{ "Parent", GetParent().GetPath() },
			// { "PosX", Position.X }, // Vector2 is not supported by JSON
			// { "PosY", Position.Y },
			{ "currentCoins", currentCoins },
			{ "levelKey", levelKey },
			{ "roomOpened", roomOpened },
			{ "foundgrapplingHook", foundgrapplingHook },
			{ "beenToLevelOne", beenToLevelOne}
		};
	}

	private void HandlePoisonTimeout()
	{
		poisoned = false;
		drain = OxygenLossRate;
		GD.Print("Poison gone!");
	}

	private void LoadPlayer()
	{
		if (loaded == false)
		{
			// GD.Print($"Current Coins before load: {currentCoins}");
			Node2D level = GetNodeOrNull<Node2D>("/root/LevelOne");
			if (level is null) level = GetNodeOrNull<Node2D>("/root/Homestead");
			if (level is not null) gameManager.LoadPlayer(level, "LevelOne"); // hard code for now?
			if (roomOpened && level.Name == "LevelOne")
			{
				hiddenRoomCovering.Visible = false;
				if (industrialWaste is not null) industrialWaste.Visible = true;
				if (hiddenRoomNote is not null) hiddenRoomNote.Visible = true;
				hiddenRoom.SetCell(new Vector2I(9, 12), 4, new Vector2I(4, 10), 1);
				hiddenRoom.SetCell(new Vector2I(9, 13), 4, new Vector2I(4, 11), 1);
				electricLock.QueueFree();
			}
			if (beenToLevelOne && displayPoison is not null) displayPoison.QueueFree();
			if (foundgrapplingHook && displayGrapplingHook is not null) displayGrapplingHook.QueueFree();
			gameManager.LoadBlueprints();
			gameManager.LoadTools();
			RebuildRecipes();
			loaded = true;
			// GD.Print($"Current Coins after load: {currentCoins}");
		}
		EmitSignal(SignalName.PlayerLoaded);
	}

	private void RebuildRecipes()
	{
		foreach (BlueprintRes entry in gameManager.knownBlueprints)
		{
			if (!gameManager.knownBlueprintNames.ContainsKey(entry.craftItemTitle))
			{
				ui.craftingMenu.AddRecpie(entry);
			}
		}
	}
}
