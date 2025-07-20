using System;
using Godot;

public partial class MainMenu : Control
{
  #region Exports
  [Export(PropertyHint.File, "*.tscn")] string StartLevel;
  [Export(PropertyHint.File, "*.tscn")] string ContinueLevel;
  #endregion

  #region Required UI (Not Exported)
  private OptionsMenu optionsMenu;
  private Button StartButton;
  private Button ContinueButton;
  private Button OptionsButton;
  private Button ExitButton;
  #endregion

  private SceneTransition sceneTransition;

  public override void _Ready()
  {
    Input.MouseMode = Input.MouseModeEnum.Visible;
    StartButton = GetNode<Button>("%StartButton");
    ContinueButton = GetNode<Button>("%ContinueButton");
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
    ContinueButton.Pressed += OnContinuePress;
    OptionsButton.Pressed += OnOptionsPress;
    ExitButton.Pressed += OnExitPress;
  }

  private void OnStartPress()
  {
    ResetPlayer();
    ResetLevel();
    if (optionsMenu.Visible == true) optionsMenu.ToggleVisable();
    sceneTransition.ChangeScene(StartLevel);
  }

  private void OnContinuePress()
  {
    if (optionsMenu.Visible == true) optionsMenu.ToggleVisable();
    sceneTransition.ChangeScene(ContinueLevel);
    GD.Print("Shouldn't see this if Start cliked");
  }

  private void OnOptionsPress() => optionsMenu.ToggleVisable();
  private void OnExitPress() => GetTree().Quit();

  private void ResetPlayer()
  {
    // Ensure the directory exists
    var dir = DirAccess.Open("user://");
    if (!dir.DirExists("Player"))
    {
      dir.MakeDir("Player");
    }

    // if (FileAccess.FileExists($"user://Player/Player.save")) 
    using var saveFile = FileAccess.Open($"user://Player/Player.save", FileAccess.ModeFlags.Write);
    var nodeData = new Godot.Collections.Dictionary<string, Variant>()
    {
      { "Filename", "res://Entities/Player/player.tscn" },
      { "Parent", "/root/LevelOne" },
      { "currentCoins", 0 },
      { "levelKey", false },
      { "roomOpened", false },
      { "foundgrapplingHook", false },
      { "beenToLevelOne", false}
    };
    var jsonString = Json.Stringify(nodeData);
    saveFile.StoreLine(jsonString);
  }

  public void ResetLevel()
  {
    String savePath = $"user://levels/LevelOne.save";
    Godot.Collections.Array<String> paths =
    [
      $"user://levels/LevelOne.save",
      $"user://blueprints.save",
      $"user://tool_equipped.save"
    ];

    foreach (String path in paths)
    {
      if (FileAccess.FileExists(path))
      {
        GD.Print($"Deleting file: {path}");
        DirAccess.RemoveAbsolute(path);
      }
    }
  }
}
