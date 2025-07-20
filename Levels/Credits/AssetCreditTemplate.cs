using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class AssetCreditTemplate : MarginContainer
{
  [Export] Label AuthorLabel;
  [Export] Label UseLabel;
  [Export] Label AssetNameLabel;
  [Export] Button URLLabel;

  public string Author { get; set; }
  public string Use { get; set; }
  public string AssetName { get; set; }
  public string URL { get; set; }

  public void SetValues(string author, string use, string assetName, string url)
  {
    AuthorLabel.Text = author;
    UseLabel.Text = use;
    AssetNameLabel.Text = assetName;
    URLLabel.Text = url;
  }

  private void OnClickUrl()
  {
    DisplayServer.ClipboardSet(URLLabel.Text);
  }
}
