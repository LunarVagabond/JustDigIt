using System;
using Godot;
public partial class CraftingMenu : Control
{
  [Export] private VBoxContainer recepiesContainer;
  [Export] private PackedScene recepieScene;

  private Texture2D itemTexture;
  private TextureRect materialSlotOne;
  private TextureRect materialSlotTwo;

  private Label MatOneLabel;
  private Label MatTwoLabel;
  private Label MatOneCost;
  private Label MatTwoCost;
  private Player player;
  private Blueprint.ItemType skillType;
  private int skillValue;

  public override void _Ready()
  {
    materialSlotOne = GetNode<TextureRect>("%MatOne");
    materialSlotTwo = GetNode<TextureRect>("%MatTwo");
    MatOneLabel = GetNode<Label>("%MatOneLabel");
    MatTwoLabel = GetNode<Label>("%MatTwoLabel");
    MatOneCost = GetNode<Label>("%MatOneCost");
    MatTwoCost = GetNode<Label>("%MatTwoCost");

    materialSlotOne.Visible = false;
    materialSlotTwo.Visible = false;
    MatOneLabel.Visible = false;
    MatTwoLabel.Visible = false;
    MatOneCost.Visible = false;
    MatTwoCost.Visible = false;
    // InitDebugRecipe();
  }


  // TODO: The arg passed in probably should be some other type like "Blueprint" or "BP" or something
  // Based on the blueprint pickup whne it happens we can add it into recipes here 
  // Also maybe we directly pass in a resrouce file instead of a whole object / scene
  // public void AddRecpie(Pickup pickup)

  // NOTE -- all of this 3x reassigning from blueprint to recipe to local values here to do things seems messy
  public void AddRecpie(BlueprintRes blueprint)
  {
    Recipe r = recepieScene.Instantiate<Recipe>();
    // NOTE: IDK why but we need to add the item to the scene THEN update the texture of it
    // I think it's because the texture is null untill the _Ready() runs for the recepieScene
    recepiesContainer.AddChild(r);
    r.Icon.Texture = blueprint.craftItemTexture;
    r.GetNode<Label>("%RecipeName").Text = blueprint.craftItemTitle;
    r.GetNode<Label>("%RecipeDescription").Text = blueprint.craftItemDescription;
    r.materialTexture1 = blueprint.materialTexture1;
    r.materialTexture2 = blueprint.materialTexture2;
    r.materialName1 = blueprint.materialName1;
    r.materialName2 = blueprint.materialName2;
    r.materialCost1 = blueprint.materialCost1;
    r.materialCost2 = blueprint.materialCost2;
    r.skillType = blueprint.craftItem;
    r.skillValue = blueprint.skillValue;
    r.RecipeSelected += HandleSelectionOfRecipe;
  }

  private void HandleSelectionOfRecipe(Recipe r)
  {
    GD.Print("func emit");
    // change the data for the given recipe
    itemTexture = r.Icon.Texture;
    skillType = r.skillType;
    skillValue = r.skillValue;
    materialSlotOne.Texture = r.materialTexture1;
    materialSlotTwo.Texture = r.materialTexture2;
    MatOneLabel.Text = r.materialName1;
    MatTwoLabel.Text = r.materialName2;
    MatOneCost.Text = $"{r.materialCost1}";
    MatTwoCost.Text = $"{r.materialCost2}";
    // Display current recipe in crafting menu
    materialSlotOne.Show();
    materialSlotTwo.Show();
    MatOneLabel.Show();
    MatTwoLabel.Show();
    MatOneCost.Show();
    MatTwoCost.Show();
  }


  public void ToggleVisable()
  {
    if (Visible)
    {
      Visible = false;
      GetTree().Paused = false;
      Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
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
    Player player = GetParent().GetParent().GetParent() as Player; // Messy but couldnt get other ways to work.
    GD.Print("Crafting Button Pushed", player, player.currentCoins);
    int cost;
    bool sufficient = int.TryParse(MatOneCost.Text, out cost);
    if (player.currentCoins >= cost)
    {
      player.currentCoins -= cost;
      GD.Print("Tool crafted and equipped!");
      player.ui.CurrentItem.Texture = itemTexture;
      if (skillType == Blueprint.ItemType.Pickaxe) // Messy
      {
        GD.Print($"Mining skill before tool equipped: {player.stats.miningSkill}");
        player.stats.miningSkill += skillValue;
        GD.Print($"Mining skill after tool equipped: {player.stats.miningSkill}");
      }
      
    }
    else
    {
      GD.Print("Insufficient Resources");
    }
  }

  private void OnClosePressed() => ToggleVisable();

  // This can be deleted or leveraged later 
  // private void InitDebugRecipe()
  // {
  //   PackedScene scene = ResourceLoader.Load<PackedScene>("res://Entities/Interactables/Oxygen/oxygen.tscn");
  //   Pickup p = scene.Instantiate<Pickup>();
  //   AddRecpie(p);
  // }
}
