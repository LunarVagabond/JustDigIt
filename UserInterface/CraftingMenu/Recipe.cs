using Godot;

public partial class Recipe : PanelContainer
{
  [Signal] public delegate void RecipeSelectedEventHandler(Recipe r);
  public TextureRect Icon;

  public override void _Ready()
  {
    Icon = GetNode<TextureRect>("HBoxContainer/Icon");
  }

  private void OnSelected()
  {
    EmitSignal(SignalName.RecipeSelected, this);
  }
}

