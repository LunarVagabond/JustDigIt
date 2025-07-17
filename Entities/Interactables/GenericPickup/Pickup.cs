using System;
using Godot;

public partial class Pickup : Area2D
{
	[Export] public PickupRes Item;
	[Export] public int Worth;

	// NOTE: Could be shared but for now this is the only thing that requires a pickup animation
	private const string PICKUP_ANIMATION = "Pickup";
	private const string DEFAULT_ANIMATION = "child/Default";
	[Export] private Sprite2D iconNode;
	[Export] protected AnimationPlayer animationPlayer;
	protected GameManager gameManager;
	protected AudioManager audioManager;
	private string tresPath = "res://Resources/Pickups/";


	public enum ItemType
	{
		Oxygen,
		Battery,
		Coin,
		Poison
	}

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		audioManager = GetNode<AudioManager>("/root/AudioManager");
	}

	// Create the resource dynamically during game actions -- I need help figuring out how to dynamically assign resource type
	// FIXME: We can go over the comment here. I think the pickup scene might be a bit convoluted (my bad haha)
	// Going to comment out for now delete after discussion (?)
	// public async void Setup(Vector2 spawnLocation, ItemType pickupType)
	// {
	// 	if (!IsInsideTree())
	// 	{
	// 		await ToSignal(this, "ready");
	// 	}
	// 	GlobalPosition = spawnLocation;

	// 	PickupRes item = pickupType switch
	// 	{
	// 		ItemType.Coin => ResourceLoader.Load($"{tresPath}Coin.tres") as PickupRes,
	// 		ItemType.Poison => ResourceLoader.Load($"{tresPath}Poison.tres") as PickupRes,
	// 		ItemType.Oxygen => ResourceLoader.Load($"{tresPath}Bubble.tres") as PickupRes,
	// 		ItemType.Battery => ResourceLoader.Load($"{tresPath}Battery.tres") as PickupRes,
	// 		_ => ResourceLoader.Load($"{tresPath}Coin") as PickupRes
	// 	};

	// 	Item = item;
	// 	iconNode.Texture = Item.icon;
	// 	animationPlayer.Play($"{DEFAULT_ANIMATION}");
	// }

	// Based on collision settings this will and should only ever be the player
	public void OnItemBodyEnter(Node2D body)
	{
		gameManager.HandlePickupCollected(Item.itemType, Worth, body); // if we pass the whole object after queue free call we error instead pass type and value
		animationPlayer.Play(PICKUP_ANIMATION);
	}

	private void PlaySoundEffect() => audioManager.PlaySfx(Item.SoundEffect);
}
