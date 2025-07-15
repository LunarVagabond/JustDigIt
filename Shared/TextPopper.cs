using Godot;

public partial class TextPopper : Area2D
{
	[Export] string DialogToShow;
	[Export] float TimeToShow;
	private void OnPlayerEntered(Node body)
	{
		if (body is Player p)
		{
			p.ui.SetDialog(DialogToShow, TimeToShow);
		}
	}
}
