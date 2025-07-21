using System;
using Godot;
using IA = CustomInputActions.InputActions;


public partial class TextPopperAndEnder : Node
{
	[Export] Timer timeToEnd;
	[Export(PropertyHint.File, "*.tscn")] string nextScene;
	private GameManager gameManager;
	private SceneTransition st;


	public override void _Ready()
	{
		timeToEnd.Timeout += HandleTrasnition;
		st = GetNode<SceneTransition>("/root/SceneTransition");
		gameManager = GetNode<GameManager>("/root/GameManager");
	}

	private void OnPlayerEnterTimerZone(Node2D player) => timeToEnd.Start();


	private void HandleTrasnition()
	{
		gameManager.SaveEquippedTools();
		gameManager.SavePlayer();
		gameManager.SaveLevel();
		gameManager.SaveBlueprints();
		
		st.ChangeScene(nextScene);
	}
}
