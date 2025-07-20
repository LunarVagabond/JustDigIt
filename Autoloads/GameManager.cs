using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Godot;
using IA = CustomInputActions.InputActions;

public partial class GameManager : Node
{
    public Player player { get; set; }
    public UserInterface ui { get; set; }
    public readonly PackedScene recipe = ResourceLoader.Load<PackedScene>("res://UserInterface/CraftingMenu/recipe.tscn");
    public readonly PackedScene pickaxe = ResourceLoader.Load<PackedScene>("res://Resources/Blueprint/PickaxeBlueprint.tscn");
    private const String GROUP_PLAYER = "Player";
    private const String GROUP_MINING = "miningLayer";
    private const String GROUP_PERSIST = "Persist";

    public List<BlueprintRes> knownBlueprints = new List<BlueprintRes>();
    // public List<String> knownBlueprintNames = new List<String>();
    public Godot.Collections.Dictionary<string, int> knownBlueprintNames = [];

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
            if (!knownBlueprintNames.ContainsKey(blueprint.craftItemTitle))
            {
                knownBlueprints.Add(blueprint);
                knownBlueprintNames[blueprint.craftItem.ToString()] = 1;
                ui.craftingMenu.AddRecpie(blueprint);
            }
            else
            {
                GD.Print("Recipe known");
            }
        }
    }

    private void CreateRecipe(BlueprintRes blueprint)
    {
        Recipe newRecipe = recipe.Instantiate<Recipe>();
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
    public void SavePlayer()
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
                // GD.Print("Attempting to save Player");

                using var saveFile = FileAccess.Open($"user://Player/Player.save", FileAccess.ModeFlags.Write);

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
                    // GD.Print("Player Save function ran");

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
    }

    public void SaveLevel()
    {
        Node2D level;
        level = GetNodeOrNull<Node2D>("/root/LevelOne");
        if (level is not null)
        {
            GD.Print(level, level.Name);
            if (level.Name == "LevelOne")
            {
                GD.Print("Attempting to save Map in LevelOne");

                using var saveFile = FileAccess.Open($"user://levels/{level.Name}.save", FileAccess.ModeFlags.Write);

                Level saveNode = level.GetTree().GetNodesInGroup(GROUP_MINING)[0] as Level;
                GD.Print("Map on Save");
                saveNode.GetEmptyCellPositionsInRect();

                // Check the node has a save function.
                if (!saveNode.HasMethod("Save"))
                {
                    GD.Print($"persistent node '{saveNode.Name}' is missing a Save() function, skipped");
                    return;
                }

                // Call the node's save function.
                GD.Print("Map Save function triggered");
                var nodeData = saveNode.Call("Save");
                GD.Print("Map Save function ran");

                // Json provides a static method to serialized JSON string.
                var jsonString = Json.Stringify(nodeData);

                // Store the save dictionary as a new line in the save file.
                saveFile.StoreLine(jsonString);
            }
        }
        else
        {
            GD.Print("HERE", level);
        }

    }

    // Save Blueprint
    public void SaveBlueprints()
    {
        if (knownBlueprintNames.Count > 0)
        {
            String filePath = "user://blueprints.save";
            using var saveFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);

            // Store the save dictionary as a new line in the save file.
            var jsonString = Json.Stringify(knownBlueprintNames);
            saveFile.StoreLine(jsonString);
        }
        else
        {
            GD.Print("No known blueprints");
        }
    }

    public void SaveEquippedTools()
    {
        String filePathEquippedTool = "user://tool_equipped.save";
        using var saveFileTool = FileAccess.Open(filePathEquippedTool, FileAccess.ModeFlags.Write);
        // Store the save dictionary as a new line in the save file.
        var jsonStringTool = Json.Stringify(player.equippedItem);
        saveFileTool.StoreLine(jsonStringTool);
    }

    // LOAD LEVEL
    public void LoadPlayer(Node2D levelNode, String levelName)
    {
        if (!FileAccess.FileExists($"user://Player/Player.save"))
        {
            GD.Print($"Error! We don't have a save to load.");
        }

        // Get The Player node in the given Level
        Player playerToLoad = levelNode.GetTree().GetFirstNodeInGroup(GROUP_PLAYER) as Player;
        // GD.Print($"Player node: {playerToLoad}");

        // Load the file line by line and process that dictionary to restore the object
        // it represents.
        using var saveFile = FileAccess.Open($"user://Player/Player.save", FileAccess.ModeFlags.Read);
        // GD.Print($"In load funcion with file: {saveFile}");

        while (saveFile.GetPosition() < saveFile.GetLength())
        {
            // GD.Print("In Load loop...");
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

            foreach (var (key, value) in nodeData)
            {
                if (key == "Filename" || key == "Parent" || key == "PosX" || key == "PosY")
                {
                    continue;
                }
                // GD.Print($"Loading -- {key}: {value}");
                playerToLoad.Set(key, value);
            }
            // GD.Print($"Coins during load: {playerToLoad.currentCoins}");
        }
    }

    public void LoadLevel(Node2D levelNode, String levelName)
    {
        if (!FileAccess.FileExists($"user://levels/{levelName}.save"))
        {
            GD.Print($"Error! We don't have a save to load.");
        }

        // Get The Player node in the given Level
        // TileMapLayer mapToLoad = levelNode.GetTree().GetFirstNodeInGroup(GROUP_MINING) as TileMapLayer;
        Level mapToLoad = levelNode.GetTree().GetFirstNodeInGroup(GROUP_MINING) as Level;
        GD.Print($"Map node: {mapToLoad}");

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

            foreach (var (key, value) in nodeData)
            {
                if (key == "Filename" || key == "Parent" || key == "PosX" || key == "PosY")
                {
                    continue;
                }
                // GD.Print($"Loading -- {key}: {value.GetType()}");
                mapToLoad.Set(key, value);
            }
            GD.Print("Map on Load");
            GD.Print(mapToLoad.emptyCells.Count);
            for (int i = 0; i < mapToLoad.emptyCells.Count - 1; i += 1)
            {
                int x = mapToLoad.emptyCellsX[i];
                int y = mapToLoad.emptyCellsY[i];
                mapToLoad.SetCell(new Vector2I(x, y), -1);
            }
        }
    }

    // }

    public void LoadBlueprints()
    {
        String filePath = "user://blueprints.save";
        if (!FileAccess.FileExists(filePath))
        {
            GD.Print("No Blueprints to load.");
            return;
        }

        using var saveFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

        while (saveFile.GetPosition() < saveFile.GetLength())
        {
            var jsonString = saveFile.GetLine();

            // Creates the helper class to interact with JSON.
            var json = new Json();
            var parseResult = json.Parse(jsonString);
            if (parseResult != Error.Ok)
            {
                GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonString} at line {json.GetErrorLine()}");
                continue;
            }

            var nodeData = new Godot.Collections.Dictionary<string, int>((Godot.Collections.Dictionary)json.Data);
            GD.Print(nodeData);
            knownBlueprintNames = nodeData;

            foreach (var (key, value) in knownBlueprintNames)
            {
                if (key == "Pickaxe")
                {
                    Blueprint newBlueprint = pickaxe.Instantiate<Blueprint>();
                    knownBlueprints.Add(newBlueprint.Item);
                    ui.craftingMenu.AddRecpie(newBlueprint.Item); // Janks a lot
                    newBlueprint.QueueFree();
                }
            }
        }
        GD.Print(knownBlueprintNames.Count());
        GD.Print(knownBlueprints.Count());
        GD.Print("Blueprints Loading");
    }

    public void LoadTools()
    {
        // Get Equipped Tool
        String filePathEquippedTool = "user://tool_equipped.save";
        // String filePathEquippedTool = "user://bluprints.save";
        if (FileAccess.FileExists(filePathEquippedTool))
        {
            GD.Print("Loading tool equipped data");
            using var saveFileTool = FileAccess.Open(filePathEquippedTool, FileAccess.ModeFlags.Read);
            while (saveFileTool.GetPosition() < saveFileTool.GetLength())
            {
                var jsonStringTool = saveFileTool.GetLine();
                var json = new Json();
                var parseResult = json.Parse(jsonStringTool);
                if (parseResult != Error.Ok)
                {
                    GD.Print($"JSON Parse Error: {json.GetErrorMessage()} in {jsonStringTool} at line {json.GetErrorLine()}");
                    continue;
                }
                GD.Print($"Tool save info: {json.Data}");
                if (json.Data.ToString() == "Pickaxe")
                {
                    player.equippedItem = json.Data.ToString();
                    Blueprint newBlueprint = pickaxe.Instantiate<Blueprint>();
                    player.ui.CurrentItem.Texture = newBlueprint.Item.craftItemTexture;
                    player.currentMiningSkill = player.stats.miningSkill + newBlueprint.Item.skillValue;
                    GD.Print($"Pickaxe loaded: player mining skill {player.stats.miningSkill} + {newBlueprint.Item.skillValue} = {player.currentMiningSkill}");
                    GD.Print($"Pickaxe loaded: player mining skill {player.currentMiningSkill}");
                    newBlueprint.QueueFree();
                }
                else
                {
                    GD.Print($"Pickaxe loaded: player mining skill {player.currentMiningSkill}");
                }
            }
        }
        else
        {
            GD.Print("No tools equipped data");
        }
    }
}
