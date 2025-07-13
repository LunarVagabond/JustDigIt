using Godot;

public partial class MainMenu : Control
{
  #region Exports
  [Export(PropertyHint.File, "*.tscn")] string NextLevel;
  #endregion

  #region Required UI (Not Exported)
  private Button StartButton;
  private Button OptionsButton;
  private Button ExitButton;
  #endregion

  private SceneTransition sceneTransition;

  public override void _Ready()
  {
    GD.Print("Loaded");
    StartButton = GetNode<Button>("%StartButton");
    OptionsButton = GetNode<Button>("%OptionsButton");
    ExitButton = GetNode<Button>("%ExitButton");
    sceneTransition = GetNodeOrNull<SceneTransition>("/root/SceneTransition");
    WireSignals();
  }

  private void WireSignals()
  {
    StartButton.Pressed += OnStartPress;
    OptionsButton.Pressed += OnOptionsPress;
    ExitButton.Pressed += OnExitPress;
  }

  private void OnStartPress() => sceneTransition.ChangeScene(NextLevel);
  private void OnOptionsPress() => GD.Print("Options don't do anything yet");
  private void OnExitPress() => GetTree().Quit();
}
