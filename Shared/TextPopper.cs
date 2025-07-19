using System;
using Godot;
using IA = CustomInputActions.InputActions;


public partial class TextPopper : Area2D
{
	[Export] string DialogToShow;
	[Export] float TimeToShow;
	[Export] public string UniqueId = "";  // can manually set or auto gen it. Used to track seen / unseen state

	private GameManager gameManager;
	private Label InteractLabel;

	private bool overlaping = false;
	private Player player;

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		InteractLabel = GetNode<Label>("InteractLabel");
		InteractLabel.Hide();
		if (string.IsNullOrEmpty(UniqueId))
		{
			// Generate a GUID once if not already set
			UniqueId = Guid.NewGuid().ToString();
			GD.Print($"[DialogTrigger] Generated ID: {UniqueId}");
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(IA.INTERACT) && overlaping)
		{
			player.ui.SetDialog(DialogToShow, TimeToShow);
		}
	}


	/// <summary>
	/// If the player has not seen a dialog we auto show it, otherwise let them interact
	/// to see what it had said. (Intersting concept)
	/// We add the dialog's unique key to the game manager so we can track what was seeen
	/// </summary>
	/// <param name="body">The entering body</param>
	private void OnPlayerEntered(Node body)
	{
		if (body is Player p)
		{
			overlaping = true;
			player = p;
			// First time seeing this dialog
			if (!gameManager.HasSeenDialog(UniqueId))
			{
				player.ui.SetDialog(DialogToShow, TimeToShow);
				gameManager.MarkDialogSeen(UniqueId);
			}
			else
			{
				InteractLabel.Show();
			}
		}
	}

	private void OnPlayerExit(Node body)
	{
		overlaping = false;
		InteractLabel.Hide();
	}

}
