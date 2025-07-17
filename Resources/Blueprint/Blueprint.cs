using Godot;
using System;

public partial class Blueprint : Area2D
{
	[Export] public BlueprintRes Item;
	[Export] protected AnimatedSprite2D animatedSprite;
	[Export] protected CollisionShape2D collisionShape;
	protected GameManager gameManager;
	protected AudioManager audioManager;
	private const string PICKUP_ANIMATION = "Pickup";

	public enum ItemType
	{
		Ladder,
		Pickaxe,
		Key
	}

	public override void _Ready()
	{
		gameManager = GetNode<GameManager>("/root/GameManager");
		audioManager = GetNode<AudioManager>("/root/AudioManager");
		BodyEntered += OnItemBodyEnter;
		animatedSprite.AnimationFinished += () => QueueFree();
	}

	public void OnItemBodyEnter(Node2D body)
	{
		SetDeferred("monitoring", false); // So cannot pickup multiple times
		gameManager.HandleBlueprintCollected(Item, body);
		PlaySoundEffect();
		animatedSprite.Play(PICKUP_ANIMATION);
	}

	private void PlaySoundEffect() => audioManager.PlaySfx(Item.SoundEffect);
}
