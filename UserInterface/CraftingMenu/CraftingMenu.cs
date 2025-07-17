using System;
using Godot;
public partial class CraftingMenu : Control
{
  [Export] private VBoxContainer recepiesContainer;
  [Export] private PackedScene recepieScene;

  private TextureRect materialSlotOne;
  private TextureRect materialSlotTwo;

  private Label MatOneLabel;
  private Label MatTwoLabel;

  public override void _Ready()
  {
    materialSlotOne = GetNode<TextureRect>("%MatOne");
    materialSlotTwo = GetNode<TextureRect>("%MatTwo");
    MatOneLabel = GetNode<Label>("%MatOneLabel");
    MatTwoLabel = GetNode<Label>("%MatTwoLabel");

    materialSlotOne.Visible = false;
    materialSlotTwo.Visible = false;
    MatOneLabel.Visible = false;
    MatTwoLabel.Visible = false;
    InitDebugRecipe();
  }


  // TODO: The arg passed in probably should be some other type like "Blueprint" or "BP" or something
  // Based on the blueprint pickup whne it happens we can add it into recipes here 
  // Also maybe we directly pass in a resrouce file instead of a whole object / scene
  public void AddRecpie(Pickup pickup)
  {
    Recipe r = recepieScene.Instantiate<Recipe>();
    // NOTE: IDK why but we need to add the item to the scene THEN update the texture of it
    // I think it's because the texture is null untill the _Ready() runs for the recepieScene
    recepiesContainer.AddChild(r);
    r.Icon.Texture = pickup.Item.icon;
    r.RecipeSelected += HandleSelectionOfRecipe;
  }

  private void HandleSelectionOfRecipe(Recipe r)
  {
    GD.Print("func emit");
    materialSlotOne.Show();
    materialSlotTwo.Show();
    MatOneLabel.Text = "Item Name One";
    MatTwoLabel.Text = "Item Name Two";
    MatOneLabel.Show();
    MatTwoLabel.Show();
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
      materialSlotOne.Hide();
      materialSlotTwo.Hide();
      MatOneLabel.Text = "";
      MatTwoLabel.Text = "";
      MatOneLabel.Hide();
      MatTwoLabel.Hide();
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

  // This can be deleted or leveraged later 
  private void InitDebugRecipe()
  {
    PackedScene scene = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/Oxygen/oxygen.tscn");
    Pickup p = scene.Instantiate<Pickup>();
    AddRecpie(p);
  }
}
