using Godot;
using IA = CustomInputActions.InputActions;
public partial class UserInterface : CanvasLayer
{
	public TextureRect CurrentItem;
	public ProgressBar OxygenBar;
	public ProgressBar LightBar;
	public Label DepthLevelLabel;
	public Label DialogLabel;
	public Label GoldCountLabel;
	public PanelContainer DialogAreaPanel;
	public DarknessEffect darknessEffect;

	private OptionsMenu optionsMenu;
	private Player player;

	public CraftingMenu craftingMenu;

	public override void _Ready()
	{
		Visible = true; // Most scenes we will hide this in the engine but we do want it in the game to show up
		CurrentItem = GetNodeOrNull<TextureRect>("%CurrentItem");
		OxygenBar = GetNodeOrNull<ProgressBar>("%OxygenBar");
		LightBar = GetNodeOrNull<ProgressBar>("%LightBar");
		DepthLevelLabel = GetNodeOrNull<Label>("%DepthLabel");
		GoldCountLabel = GetNodeOrNull<Label>("%GoldCountLabel");
		DialogLabel = GetNodeOrNull<Label>("%DialogAreaLabel");
		DialogAreaPanel = GetNodeOrNull<PanelContainer>("%DialogAreaPanel");
		darknessEffect = GetNode<DarknessEffect>("DarknessEffect");
		optionsMenu = GetNode<OptionsMenu>("MarginContainer/VMainContainer/CenterScreenContainer/OptionsMenu");
		craftingMenu = GetNode<CraftingMenu>("MarginContainer/CraftingMenu");
		player = GetParent() as Player;
		
		craftingMenu.Visible = false;
		optionsMenu.Visible = false;
		darknessEffect.Visible = true;
		DialogAreaPanel.Visible = false;

		// // Set Initial Values
		// OxygenBar.Value = player.stats.maxOxygen;
		// LightBar.Value = player.stats.maxEnergy; // can change this to saved energy
		// GoldCountLabel.Text = $"{player.currentCoins}";
		// DepthLevelLabel.Text = "Depth: Surface";

		player.PlayerLoaded += SetInitialDisplayValues;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(IA.MENU_TOGGLE)) optionsMenu.ToggleVisable();
		if (@event.IsActionPressed(IA.CRAFTING_TOGGLE)) craftingMenu.ToggleVisable();
	}


	/// <summary>
	/// Allows us to set the dialog from anywhere that has access to the User Interface.
	/// </summary>
	/// <param name="text">The string to show on screen</param>
	/// <param name="timeToLast">How long until it fades away (default 5 seconds) </param>
	public void SetDialog(string text, float timeToLast = 5)
	{
		DialogLabel.Text = text;
		DialogLabel.Modulate = new Color(1, 1, 1, 0); // fully transparent
		DialogLabel.Visible = true;
		DialogAreaPanel.Visible = true;

		var tweenIn = GetTree().CreateTween();
		tweenIn.TweenProperty(DialogLabel, "modulate:a", 1.0f, 0.3f);

		// After fade-in, wait, then fade out
		tweenIn.Finished += () =>
		{
			var timer = new Timer
			{
				WaitTime = timeToLast,
				OneShot = true
			};
			AddChild(timer);

			timer.Timeout += () =>
			{
				timer.QueueFree();

				var tweenOut = GetTree().CreateTween();
				tweenOut.TweenProperty(DialogLabel, "modulate:a", 0.0f, 0.3f);

				tweenOut.Finished += () =>
					{
						DialogAreaPanel.Visible = false;
						DialogLabel.Visible = false;
					};
			};

			timer.Start();
		};
	}

	public void SetInitialDisplayValues()
	{
		// Set Initial Values
		player = GetParent() as Player;
		OxygenBar.Value = player.stats.maxOxygen;
		LightBar.Value = player.stats.maxEnergy; // can change this to saved energy
		GoldCountLabel.Text = $"{player.currentCoins}";
		DepthLevelLabel.Text = "Depth: Surface";
	}
	// TODO: We can / should have some grand function that dictates why UI is visable when other UI is shown / hidden 
	// For example when the Crafting menu is open we should hide the Oxygen / Energy bars
}
