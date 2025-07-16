using System;
using Godot;

[GlobalClass]
public partial class Pickup : Area2D
{
	[Export] public PickupRes Item;
	[Export] public int Worth;

	// NOTE: Could be shared but for now this is the only thing that requires a pickup animation
	private const string PICKUP_ANIMATION = "Pickup";
	private const string DEFAULT_ANIMATION = "Default";
	private Sprite2D iconNode;
	protected AnimationPlayer animationPlayer;
	protected GameManager gameManager;
	protected AudioManager audioManager;
	private string tresPath = "res://Resources/Pickups/";


	public enum ItemType
	{
		Oxygen,
		Battery,
		Coin
	}

	public override void _Ready()
	{
		iconNode = GetNode<Sprite2D>("Sprite2D");
		gameManager = GetNode<GameManager>("/root/GameManager");
		audioManager = GetNode<AudioManager>("/root/AudioManager");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
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
			"oxygen" => ResourceLoader.Load($"{tresPath}Bubble.tres") as PickupRes,
			"electicitiy" => ResourceLoader.Load($"{tresPath}Battery.tres") as PickupRes,
			_ => ResourceLoader.Load($"{tresPath}Coin") as PickupRes
		};

		Item = item;
		iconNode.Texture = Item.icon;
		animationPlayer.Play($"{pickupType}/{DEFAULT_ANIMATION}");
	}

	// Based on collision settings this will and should only ever be the player
	public void OnItemBodyEnter(Node2D body)
	{
		gameManager.HandlePickupCollected(Item.itemType, Worth); // if we pass the whole object after queue free call we error instead pass type and value
		animationPlayer.Play(PICKUP_ANIMATION);
	}

	private void PlaySoundEffect() => audioManager.PlaySfx(Item.SoundEffect);
}
