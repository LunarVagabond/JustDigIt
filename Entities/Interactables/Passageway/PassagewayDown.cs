using System.Linq;
using Godot;
using IA = CustomInputActions.InputActions;

/*
    Dev Note:
    We the texture rect is place holder and we can probably use an actual image the detection logic will remain the same though
*/
public partial class PassagewayDown : Node2D
{
    [Export(PropertyHint.File, "*.tscn")] private string NextLevel;

    private Label InteractButtonLabel;
    private SceneTransition sceneTransition => GetNodeOrNull<SceneTransition>("/root/SceneTransition");

    private bool isOverlapping = false; // FIXME: this is a messy way of doing this but hey it works for now
    private Player player;

    public override void _Ready()
    {
        InteractButtonLabel = GetNode<Label>("InteractionLabel");
        InteractButtonLabel.Visible = false;
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(IA.INTERACT) && isOverlapping)
        {
            player.gameManager.SavePlayer();
            player.gameManager.SaveLevel();
            player.gameManager.SaveBlueprints();
            player.gameManager.SaveEquippedTools();
            sceneTransition.ChangeScene(NextLevel);   
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        InteractButtonLabel.Visible = body is Player ? true : false;
        player = body as Player;
        isOverlapping = true;
    }
    private void OnBodyExited(Node2D body)
    {
        InteractButtonLabel.Visible = false;
        isOverlapping = false;
    }
}
