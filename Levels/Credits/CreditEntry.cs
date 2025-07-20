public class CreditEntry
{
  public string AssetName { get; set; }
  public string URL { get; set; }
  public string Author { get; set; }
  public string Use { get; set; }

  public CreditEntry(string assetName, string url, string author, string use)
  {
    AssetName = assetName;
    URL = url;
    Author = author;
    Use = use;
  }
}