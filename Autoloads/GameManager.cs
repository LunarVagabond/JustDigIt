using System;
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
    private const String GROUP_PLAYER = "Player";
    private const String GROUP_PERSIST = "Persist";

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

    // SAVE LEVEL
    // Copied from: https://docs.godotengine.org/en/stable/tutorials/io/saving_games.html
    // public void SaveLevel(Node2D levelNode, String levelName)
    public void SaveLevel()
    {
        Node2D level;
        level = GetNodeOrNull<Node2D>("/root/LevelOne");
        if (level is null) level = GetNodeOrNull<Node2D>("/root/Homestead");
        if (level is not null)
        {
            GD.Print(level, level.Name);
            if (level.Name == "LevelOne")
            {
                player.beenToLevelOne = true;
                GD.Print("Attempting to save Player in LevelOne");

                using var saveFile = FileAccess.Open($"user://levels/{level.Name}.save", FileAccess.ModeFlags.Write);

                var saveNodes = level.GetTree().GetNodesInGroup(GROUP_PLAYER);
                foreach (Node saveNode in saveNodes)
                {
                    // Check the node is an instanced scene so it can be instanced again during load.
                    if (string.IsNullOrEmpty(saveNode.SceneFilePath))
                    {
                        GD.Print($"persistent node '{saveNode.Name}' is not an instanced scene, skipped");
                        continue;
                    }

                    // Check the node has a save function.
                    if (!saveNode.HasMethod("Save"))
                    {
                        GD.Print($"persistent node '{saveNode.Name}' is missing a Save() function, skipped");
                        continue;
                    }

                    // Call the node's save function.
                    var nodeData = saveNode.Call("Save");
                    GD.Print("Save function ran");

                    // Json provides a static method to serialized JSON string.
                    var jsonString = Json.Stringify(nodeData);

                    // Store the save dictionary as a new line in the save file.
                    saveFile.StoreLine(jsonString);
                }
            }
        }
        else
        {
            GD.Print("HERE", level);
        }

        // using var saveFile = FileAccess.Open($"user://levels/{levelName}.save", FileAccess.ModeFlags.Write);

        // var saveNodes = levelNode.GetTree().GetNodesInGroup(GROUP_PLAYER);
        // foreach (Node saveNode in saveNodes)
        // {
        //     // Check the node is an instanced scene so it can be instanced again during load.
        //     if (string.IsNullOrEmpty(saveNode.SceneFilePath))
        //     {
        //         GD.Print($"persistent node '{saveNode.Name}' is not an instanced scene, skipped");
        //         continue;
        //     }

        //     // Check the node has a save function.
        //     if (!saveNode.HasMethod("Save"))
        //     {
        //         GD.Print($"persistent node '{saveNode.Name}' is missing a Save() function, skipped");
        //         continue;
        //     }

        //     // Call the node's save function.
        //     var nodeData = saveNode.Call("Save");
        //     GD.Print("Save function ran");

        //     // Json provides a static method to serialized JSON string.
        //     var jsonString = Json.Stringify(nodeData);

        //     // Store the save dictionary as a new line in the save file.
        //     saveFile.StoreLine(jsonString);
        // }
    }

    // LOAD LEVEL
    public void LoadLevel(Node2D levelNode, String levelName)
    {
        if (!FileAccess.FileExists($"user://levels/{levelName}.save"))
        {
            GD.Print($"Error! We don't have a save to load.");
            return; // Error! We don't have a save to load.
        }

        // We need to revert the game state so we're not cloning objects during loading.
        // This will vary wildly depending on the needs of a project, so take care with
        // this step.
        // For our example, we will accomplish this by deleting saveable objects.
        // var saveNodes = levelNode.GetTree().GetNodesInGroup(GROUP_PLAYER);
        var saveNodes = levelNode.GetTree().GetFirstNodeInGroup(GROUP_PLAYER) as Player;
        GD.Print($"Player node: {saveNodes}");
        // foreach (Node saveNode in saveNodes)
        // {
        //     GD.Print($"Deleting node: {saveNode}");
        //     saveNode.QueueFree();
        // }

        // Load the file line by line and process that dictionary to restore the object
        // it represents.
        using var saveFile = FileAccess.Open($"user://levels/{levelName}.save", FileAccess.ModeFlags.Read);
        GD.Print($"In load funcion with file: {saveFile}");

        while (saveFile.GetPosition() < saveFile.GetLength())
        {
            GD.Print("In Load loop...");
            var jsonString = saveFile.GetLine();

            // Creates the helper class to interact with JSON.
            var json = new Json();
            var parseResult = json.Parse(jsonString);
            if (parseResult != Error.Ok)
            {
                GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
                continue;
            }

            // Get the data from the JSON object.
            var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);

            // Firstly, we need to create the object and add it to the tree and set its position.
            // var newObjectScene = GD.Load<PackedScene>(nodeData["Filename"].ToString());
            // var newObject = newObjectScene.Instantiate<Node>();
            // var newObject = newObjectScene.Instantiate<Player>();
            // GetNode(nodeData["Parent"].ToString()).CallDeferred("add_child", newObject);
            // GetNode(nodeData["Parent"].ToString()).AddChild(newObject);
            // newObject.Set(Node2D.PropertyName.Position, new Vector2((float)nodeData["PosX"], (float)nodeData["PosY"]));
            // GD.Print($"Created node: {newObject}");
            // Now we set the remaining variables.
            foreach (var (key, value) in nodeData)
            {
                if (key == "Filename" || key == "Parent" || key == "PosX" || key == "PosY")
                {
                    continue;
                }
                // newObject.Set(key, value);
                GD.Print($"Loading -- {key}: {value}");
                saveNodes.Set(key, value);
            }
            GD.Print($"Coins during load: {saveNodes.currentCoins}");
        }
    }
}
