using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class BlueprintRes : Resource
{
    [Export] public AudioStream SoundEffect;
    [Export] public  Blueprint.ItemType craftItem;
    [Export] public  String craftItemTitle;
    [Export] public AtlasTexture craftItemTexture;
    [Export] public String craftItemDescription;
    [Export] public int skillValue;
    [Export] public Texture2D materialTexture1;
    [Export] public Texture2D materialTexture2;
    [Export] public String materialName1;
    [Export] public String materialName2;
    [Export] public int materialCost1;
    [Export] public int materialCost2;


    // This may be better long term, but felt like a lot to do right now...
    // public Dictionary<String, dynamic> craftingLookup = new Dictionary<string, dynamic>
    // {
    //     {
    //         "Ladder", new Dictionary<String, dynamic>
    //         {
    //             { "craftItemTitle", "Ladder" }
    //             { ""}
    //         }
    //     }
    // };

}