using Godot;

public partial class DarknessEffect : Control
{
	ColorRect shaderNode;
	ShaderMaterial mat;

	public override void _Ready()
	{
		shaderNode = GetNode<ColorRect>("DarknessOverlay");
		mat = (ShaderMaterial)shaderNode.Material;
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
	public void UpdateDarknessVerySmall(Vector2 playerWorldPos) => UpdateDarkness(playerWorldPos, 0.1f);

	/// <summary>Update darkness with a small visible radius around the player.</summary>
	public void UpdateDarknessSmall(Vector2 playerWorldPos) => UpdateDarkness(playerWorldPos, 0.2f);

	/// <summary>Update darkness with a medium visible radius around the player.</summary>
	public void UpdateDarknessMed(Vector2 playerWorldPos) => UpdateDarkness(playerWorldPos, 0.3f);

	/// <summary>Update darkness with a large visible radius around the player.</summary>
	public void UpdateDarknessLarge(Vector2 playerWorldPos) => UpdateDarkness(playerWorldPos, 0.7f);

}
