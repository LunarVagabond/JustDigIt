using Godot;

public partial class MainMenu : Control
{

  private Button StartButton;
  private Button OptionsButton;
  private Button ExitButton;

  public override void _Ready()
  {
    StartButton = GetNode<Button>("%StartButton");
    OptionsButton = GetNode<Button>("%OptionsButton");
    ExitButton = GetNode<Button>("%ExitButton");
    WireSignals();
  }

  private void WireSignals()
  {
    StartButton.Pressed += OnStartPress;
    OptionsButton.Pressed += OnOptionsPress;
    ExitButton.Pressed += OnExitPress;
  }

  private void OnStartPress() => GD.Print("Starting the Game");
  private void OnOptionsPress() => GD.Print("Options don't do anything yet");
  private void OnExitPress() => GetTree().Quit();
}
