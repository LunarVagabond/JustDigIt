using System.Collections.Generic;
using Godot;
using IA = CustomInputActions.InputActions;


public partial class DarknessEffect : Control
{
	ColorRect shaderNode;
	ShaderMaterial mat;
	SightRadius currentSightRadius = SightRadius.Off;
	bool forceMinimalSight = false;

	private enum SightRadius
	{
		VerySmall,
		Small,
		Medium,
		Large,
		Off // Must always be the last one 
	}

	private Dictionary<SightRadius, float> SightMappings = new Dictionary<SightRadius, float>{
		{SightRadius.VerySmall, 0.1f},
		{SightRadius.Small, 0.2f},
		{SightRadius.Medium, .5f},
		{SightRadius.Large, 0.7f},
		{SightRadius.Off, 1f},

	};

	public override void _Ready()
	{
		shaderNode = GetNode<ColorRect>("DarknessOverlay");
		mat = (ShaderMaterial)shaderNode.Material;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(IA.TURN_LAMP_UP))
		{
			IncreaseSightRadius();
		}
		if (@event.IsActionPressed(IA.TURN_LAMP_DOWN))
		{
			DecreaseSightRadius();
		}
	}


	public void HandleEnergyDrain(ProgressBar bar, Player player, Vector2 playerPos)
	{
		if ((player.currentEnergy == 0 || forceMinimalSight) && currentSightRadius != SightRadius.Off)
		{
			currentSightRadius = SightRadius.VerySmall;
			UpdateDarknessVerySmall(playerPos);
			return;
		}
		float amountDrain = 0f; // FIXME: Magic numbers are bad 
		switch (currentSightRadius)
		{
			case SightRadius.VerySmall:
				// Lamp is off but we need darkness
				amountDrain = 0;
				UpdateDarknessVerySmall(playerPos);
				break;
			case SightRadius.Small:
				amountDrain = .02f; // FIXME: Magic numbers are bad 
				UpdateDarknessSmall(playerPos);

				break;
			case SightRadius.Medium:
				amountDrain = .04f; // FIXME: Magic numbers are bad 
				UpdateDarknessMed(playerPos);

				break;
			case SightRadius.Large:
				amountDrain = .05f; // FIXME: Magic numbers are bad 
				UpdateDarknessLarge(playerPos);
				break;
			case SightRadius.Off:
				// Lamp is off no drain OR there is no need for this to be on at the current frame
				amountDrain = 0; // FIXME: Magic numbers are bad 
				break;
			default:
				GD.Print("Probably should have handled this case with off (idk)");
				break;
		}
		bar.Value -= amountDrain;
		player.currentEnergy = (float)bar.Value;
	}

	/// <summary>
	/// Updates the darkness shader parameters based on the player's world position and visibility radius.
	/// </summary>
	/// <param name="playerWorldPos">The player's position in world coordinates.</param>
	/// <param name="radius">The radius of the visible (clear) area around the player, in normalized screen space.
	/// The radius should be kept between 0 (darkest / full fill) and 1 (brightest / no effect) </param>
	public void UpdateDarkness(Vector2 playerWorldPos, float radius)
	{
		// Get the active 2D camera from the current viewport
		var camera = GetViewport().GetCamera2D();

		// Get the viewport's visible rectangle size in pixels (screen resolution)
		Vector2 screenSize = GetViewport().GetVisibleRect().Size;

		// Calculate the screen's aspect ratio (width divided by height)
		float aspectRatio = screenSize.X / screenSize.Y;

		// Convert player's world position to a position relative to the camera
		Vector2 relativePos = playerWorldPos - camera.GlobalPosition;

		// Translate relative position into screen space by offsetting by half the screen size
		Vector2 screenPos = (screenSize * 0.5f) + relativePos;

		// Normalize screen position to UV coordinates (0 to 1 range)
		Vector2 uv = screenPos / screenSize;

		// Update shader parameters:
		// - Player position in normalized screen coordinates
		// - Radius of clear area
		// - Aspect ratio for proper circle scaling in shader
		mat.SetShaderParameter("player_screen_pos", uv);
		mat.SetShaderParameter("radius", radius);
		mat.SetShaderParameter("aspect_ratio", aspectRatio);
	}

	/// <summary>Update darkness with a very small visible radius around the player.</summary>
	public void UpdateDarknessVerySmall(Vector2 playerWorldPos)
	{
		currentSightRadius = SightRadius.VerySmall;
		UpdateDarkness(playerWorldPos, SightMappings[SightRadius.VerySmall]);
	}

	/// <summary>Update darkness with a small visible radius around the player.</summary>
	public void UpdateDarknessSmall(Vector2 playerWorldPos)
	{
		currentSightRadius = SightRadius.Small;
		UpdateDarkness(playerWorldPos, SightMappings[SightRadius.Small]);
	}

	/// <summary>Update darkness with a medium visible radius around the player.</summary>
	public void UpdateDarknessMed(Vector2 playerWorldPos)
	{
		currentSightRadius = SightRadius.Medium;
		UpdateDarkness(playerWorldPos, SightMappings[SightRadius.Medium]);
	}

	/// <summary>Update darkness with a large visible radius around the player.</summary>
	public void UpdateDarknessLarge(Vector2 playerWorldPos)
	{
		currentSightRadius = SightRadius.Large;
		UpdateDarkness(playerWorldPos, SightMappings[SightRadius.Large]);
	}
	// NOTE: at some point we should have this effect off at level 1-10, toggle on past this and solwly increase the effect
	public void ToggleDarknessEffect()
	{
		Visible = !Visible;
		currentSightRadius = Visible ? SightRadius.Off : currentSightRadius;
	}

	private void IncreaseSightRadius()
	{
		if (forceMinimalSight)
			return; // Can't increase while energy is depleted

		// Convert enum to int and increase, clamping at max
		int newValue = Mathf.Min((int)currentSightRadius + 1, (int)SightRadius.Large);
		currentSightRadius = (SightRadius)newValue;
	}

	private void DecreaseSightRadius()
	{
		// Convert enum to int and decrease, clamping at min
		int newValue = Mathf.Max((int)currentSightRadius - 1, (int)SightRadius.VerySmall);
		currentSightRadius = (SightRadius)newValue;
	}
}
