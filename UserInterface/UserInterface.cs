using Godot;
using System;
using System.Threading.Tasks;

public partial class UserInterface : Control
{
	private TextureRect CurrentItemTexture;
	private ProgressBar OxygenBar;
	private ProgressBar LightBar;
	private Label DepthLevelLabel;
	private Label DialogLabel;
	private Label GoldCountLabel;
	private PanelContainer DialogAreaPanel;

	public override void _Ready()
	{
		Visible = true; // Most scenes we will hide this in the engine but we do want it in the game to show up
		CurrentItemTexture = GetNodeOrNull<TextureRect>("%CurrentItem");
		OxygenBar = GetNodeOrNull<ProgressBar>("%OxygenBar");
		LightBar = GetNodeOrNull<ProgressBar>("%LightBar");
		DepthLevelLabel = GetNodeOrNull<Label>("%DepthLabel");
		GoldCountLabel = GetNodeOrNull<Label>("%GoldCountLabel");
		DialogLabel = GetNodeOrNull<Label>("%DialogAreaLabel");
		DialogAreaPanel = GetNodeOrNull<PanelContainer>("%DialogAreaPanel");
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
}
