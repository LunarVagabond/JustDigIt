using System;
using Godot;

public partial class Pickup : Area2D
{
	[Export] public PickupRes Item;
	[Export] public int Worth;

	private TextureRect iconNode;
	protected PickupEvents pickupEventsGlobal;
	private String tresPath = "res://Resources/Pickups/";

	public enum ItemType
	{
		Oxygen,
		Battery,
		Coin
	}

	public override void _Ready()
	{
		iconNode = GetNode<TextureRect>("Control/ItemIcon");
		pickupEventsGlobal = GetNode<PickupEvents>("/root/PickupEvents");
		if (Item is not null) // Errors for dynamically loaded pickups
		{
			iconNode.Texture = Item.icon;	
		}
	}

	// Create the resource dynamically during game actions -- I need help figuring out how to dynamically assign resource type
	public async void Setup(Vector2 spawnLocation, String pickupType)
	{
		if (!IsInsideTree())
		{
			await ToSignal(this, "ready");
		}
		GlobalPosition = spawnLocation;

		PickupRes item = pickupType switch
		{
			"coin" => ResourceLoader.Load($"{tresPath}Coin.tres") as PickupRes,
			"poison" => ResourceLoader.Load($"{tresPath}Poison.tres") as PickupRes,
			"oxygen" => ResourceLoader.Load($"{tresPath}Oxygen.tres") as PickupRes,
			"electicitiy" => ResourceLoader.Load($"{tresPath}Battery.tres") as PickupRes,
			_ => ResourceLoader.Load($"{tresPath}Coin") as PickupRes
		};

		Item = item;
		iconNode.Texture = Item.icon;
	}

	// Based on collision settings this will and should only ever be the player
	public void OnItemBodyEnter(Node2D body)
	{
		pickupEventsGlobal.EmitSignal(PickupEvents.SignalName.PickupCollected, this, body);
		QueueFree();
	}
}
