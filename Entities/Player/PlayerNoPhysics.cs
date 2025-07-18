using Godot;


public partial class PlayerNoPhysics : CharacterBody2D
{
	[Export] RichTextLabel dialog;

	[Export] public float LetterDelay = 0.05f;
	[Export] AnimationPlayer animationPlayer;
	private string fullText = "";

	private SceneTransition sceneTransition;
	private AnimatedSprite2D playerSprite; // TODO: fix sprite or change, then FlipH

	public enum AnimState { Idle, Run, Climb, Death, Jump, }

	public AnimState CurrentState = AnimState.Idle;

	public GameManager gameManager;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		sceneTransition = GetNodeOrNull<SceneTransition>("/root/SceneTransition");
		sceneTransition.FadeIn();
		playerSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		animationPlayer.Play("WalkToInitialSpot");
	}



	private void SetAnimationState(AnimState state)
	{
		playerSprite.Play(state.ToString().ToLower());
	}
	public async void ShowText(string text)
	{
		fullText = text;
		dialog.Text = "asdfa";
		dialog.VisibleCharacters = 0;

		dialog.Text = fullText;
		GD.Print(fullText);
		await ToSignal(GetTree(), "process_frame");

		for (int i = 0; i <= fullText.Length; i++)
		{
			dialog.VisibleCharacters = i;
			await ToSignal(GetTree().CreateTimer(LetterDelay), "timeout");
		}
	}

	private void BeginFadeOut() => sceneTransition.EarlyFadeOut();

	private void MoveToHomestead() => sceneTransition.ChangeScene("res://Levels/Homestead/homestead.tscn");
}
