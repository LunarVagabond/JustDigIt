using Godot;
using System;
using System.Runtime.Serialization;

public partial class Recipe : PanelContainer
{
  private void OnSelected()
  {
    GD.Print("This button should trigger the RightSidePanel in CraftinMenu scene to populate with _stuff_");
  }
}
