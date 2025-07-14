using Godot;

public partial class SceneTransition : CanvasLayer
{

  private AudioManager audioManager;
  ColorRect fader;
  public override void _Ready()
  {
    fader = GetNode<ColorRect>("Fader");
    audioManager = GetNodeOrNull<AudioManager>("/root/AudioManager");
  }

  public void FadeIn()
  {
    Tween tween = GetTree().CreateTween();
    tween.TweenInterval(0.1f);
    tween.TweenProperty(fader, "color:a", 0.0, 1.0f).From(1f);
    audioManager.ResumeMusic();
  }

  public void ChangeScene(string nextScene)
  {
    audioManager.StopMusic();
    Tween tween = GetTree().CreateTween();
    tween.TweenProperty(fader, "color:a", 1.0, 1.0f);
    tween.TweenInterval(0.1f);
    tween.TweenCallback(
      Callable.From(
        () =>
        {
          GetTree().ChangeSceneToFile(nextScene);
        }
      )
    );
  }
}
