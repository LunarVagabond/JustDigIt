using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using IA = CustomInputActions.InputActions;

public partial class GameManager : Node
{
    public Player player { get; set; }
    // private CraftingMenu craftingMenu;
    public UserInterface ui { get; set; }
    public readonly PackedScene recipe = ResourceLoader.Load<PackedScene>("res://UserInterface/CraftingMenu/recipe.tscn");

    // Track dialog key's we've seen (Funny enough learned this idea from: https://www.youtube.com/shorts/EP-fIhAe2Jo)
    // Inbound Shovel's YT
    private HashSet<string> dialogsSeen = new HashSet<string>();


    // NOTE: Moved this out from player so ALL scenes get this interaction by default
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else if (@event.IsActionPressed(IA.TOGGLE_MOUSE_VISIBILITY))
        {
            if (Input.MouseMode == Input.MouseModeEnum.Visible)
            {
                Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
            }
            else
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
        }
    }

    // TODO: Handle different types here and add to the player stats or something in this
    public void HandlePickupCollected(Pickup.ItemType itemType, float value, Node2D body)
    {
        if (itemType == Pickup.ItemType.Coin && body is Player player)
        {
            GD.Print("Coin Gathered!");
            player.currentCoins += 1;
        }
    }

    public void HandleBlueprintCollected(BlueprintRes blueprint, Node2D body)
    {
        if (body is Player player)
        {
            GD.Print($"Blueprint {blueprint.craftItem} Gathered!");
            ui.craftingMenu.AddRecpie(blueprint);
        }
    }

    private void CreateRecipe(BlueprintRes blueprint)
    {
        Recipe newRecipe = recipe.Instantiate<Recipe>();
        // Pickup newPickup = coin.Instantiate<Pickup>();
        // newRecipe.Icon = (TextureRect) blueprint.craftItemTexture; // Prob need to change recipe scene
        // Should be give recipe a resource that this just becomes?
        newRecipe.GetNode<Label>("%RecipeName").Text = blueprint.craftItemTitle;
        newRecipe.GetNode<Label>("%RecipeDescription").Text = blueprint.craftItemDescription;
        GD.Print(newRecipe.GetNode<Label>("%RecipeName").Text);
        AddChild(newRecipe); // Does this need to be added to the Crafting Menu?
    }

    // We can probably (?) use this to access the player safely. 
    // TBH though it may be overkill an not needed with how player gets setup now
    private async Task<Player> AccessPlayer()
    {
        if (player is null) await ToSignal(player, "ready");
        return player;
    }

    // If seen return true otherwise false
    public bool HasSeenDialog(string uuid) => dialogsSeen.Contains(uuid) ? true : false;
    public void MarkDialogSeen(string uuid) => dialogsSeen.Add(uuid);
}
