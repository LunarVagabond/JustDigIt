using System.Collections.Generic;
using Godot;

public partial class Credits : Control
{
  [Export] PackedScene CreditTemplateScene;
  // NOTE: This is a hardcoded list if we add more this list MUST grow. We should move to yaml or json later but for 10 day jam let's send it
  public List<CreditEntry> credits = new List<CreditEntry>
  {
    new CreditEntry("ui-user-interface-mega-pack", "https://toffeecraft.itch.io/ui-user-interface-mega-pack", "ToffeeCraft", "All UI for banners and interface styling"),
    new CreditEntry("complete-ui-essential-pack", "https://crusenho.itch.io/complete-ui-essential-pack", "Crusenho", "All UI for banners and interface styling"),
    new CreditEntry("free-tiny-hero-sprites-pixel-art", "https://free-game-assets.itch.io/free-tiny-hero-sprites-pixel-art", "Free Game Assets (GUI, Sprite, Tilesets)", "Player sprite"),
    new CreditEntry("basic-pixel-health-bar-and-scroll-bar", "https://bdragon1727.itch.io/basic-pixel-health-bar-and-scroll-bar", "bdragon1727", "UI Bars"),
    new CreditEntry("Your unsettling story [background music]", "https://pixabay.com//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=228420", "ALIENIGHTMARE", "Background Music"),
    new CreditEntry("Weapon Axe Hit 01", "https://pixabay.com/sound-effects//?utm_source=link-attribution&utm_medium=referral&utm_campaign=music&utm_content=153372", "imagne_impossible", "Digging Sound Effect"),
    new CreditEntry("Bubble", "https://cassala.itch.io/bubble-sprites", "cassala", "Oxygen Bubble drop"),
    new CreditEntry("free-pixel-art-sidescroller-asset-pack-32x32-overworld", "https://gandalfhardcore.itch.io/free-pixel-art-sidescroller-asset-pack-32x32-overworld", "GandalfHardcore", "Homestead Scene"),
    new CreditEntry("magical-book", "https://nyknck.itch.io/magical-book", "Nynck", "Blueprint"),
    new CreditEntry("pixel-art-effectfx010", "https://nyknck.itch.io/pixel-art-effectfx010", "Nynck", "Poison Gas"),
    new CreditEntry("iron", "https://gamer247.itch.io/rpg", "Gamer247", "pickaxe"),
    new CreditEntry("cave-tileset", "https://grafxkid.itch.io/cave-tileset", "GrafxKid", "Level Mining Layer"),
    new CreditEntry("sidescroller-shooter-central-city", "https://anokolisa.itch.io/sidescroller-shooter-central-city", "Anokolisa", "Hidden Room"),
    new CreditEntry("dungeon-platformer-tile-set-pixel-art", "https://incolgames.itch.io/dungeon-platformer-tile-set-pixel-art", "DavidG aka IncoloGames", "Hidden Room Walls"),
    new CreditEntry("brackeys-platformer-bundle", "https://brackeysgames.itch.io/brackeys-platformer-bundle", "Brackeys", "Cracked tiles and hidden key tile, coins"),
    new CreditEntry("free-industrial-zone-tileset-pixel-art", "https://free-game-assets.itch.io/free-industrial-zone-tileset-pixel-art", "Free Game Assets", "Industrial Waste and Note Board in Hidden Room"),
    new CreditEntry("castle-platformer-tileset-16x16free", "https://rottingpixels.itch.io/castle-platformer-tileset-16x16free", "Rotting Pixels", "Ladder, ladder platform, hidden key"),
    new CreditEntry("underground-mine", "https://nicopardo.itch.io/underground-mine", "NicoPardo", "Mine Level Background")
  };

  private VBoxContainer creditsContainer;

  public override void _Ready()
  {
    creditsContainer = GetNode<VBoxContainer>("%CreditContainer");
    foreach (CreditEntry entry in credits)
    {
      AssetCreditTemplate newCredit = CreditTemplateScene.Instantiate<AssetCreditTemplate>();
      newCredit.SetValues(entry.Author, entry.Use, entry.AssetName, entry.URL);
      creditsContainer.AddChild(newCredit);
    }
  }

  private void OnClosePressed() => Visible = false;

}
