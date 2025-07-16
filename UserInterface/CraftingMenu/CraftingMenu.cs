using Godot;
public partial class CraftingMenu : Control
{
  [Export] private VBoxContainer recepiesContainer;
  [Export] private PackedScene recepieScene;


  // TODO: The arg passed in probably should be some other type like "Blueprint" or "BP" or something
  // Based on the blueprint pickup whne it happens we can add it into recipes here 
  public void AddRecpie(Pickup pickup)
  {
    Recipe r = recepieScene.Instantiate<Recipe>();
    // NOTE: IDK why but we need to add the item to the scene THEN update the texture of it
    // I think it's because the texture is null untill the _Ready() runs for the recepieScene
    recepiesContainer.AddChild(r);
    r.Icon.Texture = pickup.Item.icon;
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
