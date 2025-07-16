using Godot;

public partial class Recipe : PanelContainer
{
  public TextureRect Icon;

  public override void _Ready()
  {
    Icon = GetNode<TextureRect>("HBoxContainer/Icon");
    GD.Print(Icon);
  }

  private void OnSelected()
  {
    GD.Print("This button should trigger the RightSidePanel in CraftinMenu scene to populate with _stuff_");
  }
}

