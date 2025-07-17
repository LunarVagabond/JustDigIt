using System;
using Godot;

public partial class Recipe : PanelContainer
{
  [Signal] public delegate void RecipeSelectedEventHandler(Recipe r);
  public TextureRect Icon;
  
  public Texture2D materialTexture1;
  public Texture2D materialTexture2;
  public String materialName1;
  public String materialName2;
  public int materialCost1;
  public int materialCost2;

  public override void _Ready()
  {
    Icon = GetNode<TextureRect>("HBoxContainer/Icon");
  }

  private void OnSelected()
  {
    EmitSignal(SignalName.RecipeSelected, this);
  }
}

