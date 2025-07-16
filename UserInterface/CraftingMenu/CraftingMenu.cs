using Godot;
using IA = CustomInputActions.InputActions;

public partial class CraftingMenu : Control
{

  public override void _Ready()
  {
  }

  public void ToggleVisable()
  {
    if (Visible)
    {
      Visible = false;
      GetTree().Paused = false;
      Input.MouseMode = Input.MouseModeEnum.Captured;
    }
    else
    {
      Visible = true;
      GetTree().Paused = true;
      Input.MouseMode = Input.MouseModeEnum.Visible;
    }
  }

  private void OnCrafterPressed()
  {
    GD.Print("Crafting Button Pushed");
  }

  private void OnClosePressed() => ToggleVisable();
}
