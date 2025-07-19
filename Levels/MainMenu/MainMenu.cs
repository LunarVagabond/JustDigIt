using System;
using Godot;

public partial class MainMenu : Control
{
  #region Exports
  [Export(PropertyHint.File, "*.tscn")] string NextLevel;
  #endregion

  #region Required UI (Not Exported)
  private OptionsMenu optionsMenu;
  private Button StartButton;
  private Button OptionsButton;
  private Button ExitButton;
  #endregion

  private SceneTransition sceneTransition;

  public override void _Ready()
  {
    Input.MouseMode = Input.MouseModeEnum.Visible;
    StartButton = GetNode<Button>("%StartButton");
    OptionsButton = GetNode<Button>("%OptionsButton");
    ExitButton = GetNode<Button>("%ExitButton");
    sceneTransition = GetNodeOrNull<SceneTransition>("/root/SceneTransition");
    optionsMenu = GetNode<OptionsMenu>("MarginContainer/VBoxContainer/ButtonsContainer/OptionsMenu"); // Maybe use unique name here
    optionsMenu.Visible = false;
    sceneTransition.FadeIn();
    WireSignals();
  }

  private void WireSignals()
  {
    StartButton.Pressed += OnStartPress;
    OptionsButton.Pressed += OnOptionsPress;
    ExitButton.Pressed += OnExitPress;
  }

  private void OnStartPress()
  {
    // String levelName = "LevelOne";
    ResetPlayer();
    ResetLevel();
    // ResetHiddenRoom();
    if (optionsMenu.Visible == true) optionsMenu.ToggleVisable();
    sceneTransition.ChangeScene(NextLevel);
  }
  private void OnOptionsPress() => optionsMenu.ToggleVisable();
  private void OnExitPress() => GetTree().Quit();

  private void ResetPlayer()
  {
    if (FileAccess.FileExists($"user://Player/Player.save"))
    {
      using var saveFile = FileAccess.Open($"user://Player/Player.save", FileAccess.ModeFlags.Write);
      var nodeData = new Godot.Collections.Dictionary<string, Variant>()
      {
        { "Filename", "res://Entities/Player/player.tscn" },
        { "Parent", "/root/LevelOne" },
        { "currentCoins", 0 },
        { "levelKey", false },
        { "beenToLevelOne", false}
      };
      var jsonString = Json.Stringify(nodeData);
      saveFile.StoreLine(jsonString);
    }
  }

  public void ResetLevel()
  {
    String savePath = $"user://levels/LevelOne.save";
    // if (ResourceLoader.Exists(savePath))
    if (FileAccess.FileExists(savePath))
    {
      GD.Print("Deleting file");
      DirAccess.RemoveAbsolute(savePath);
    }
  }

}
