using Godot;
using System;

[GlobalClass]
public partial class CharacterStats : Resource
{
    [Signal]
    public delegate void LevelUpNotificationEventHandler();

    public int coins = 0;
    public float oxygen = 100.0f; // rather than increasing with XP, skills/equipment will slow down loss
    public float energy = 100.0f; // rather than increasing with XP, skills/equipment will slow down loss
    // public int skillPoints = 0; // will eventually use to choose skill upgrades
    public int crafting = 1;
    public int mining = 1;
    public int zone; // world level player is in
    public float PoisonEffect = 2.0f;
    public float poisonDuration = 5.0f;
    private int level = 1; // player exp level
    private float baseXP = 50.0f;
    private float increase20perc = 1.2f;
    private int _xp = 0;
    public int XP
    {
        get => _xp;
        set
        {
            _xp = value;
            int boundary = PercentageLevelUpBoundary();

            while (_xp >= boundary)
            {
                _xp -= boundary;
                LevelUp();
                boundary = PercentageLevelUpBoundary();
            }

            GD.Print($"\nCurrent Level: {level}");
            GD.Print($"XP needed for next level: {boundary}");
        }
    }

    public int PercentageLevelUpBoundary()
    {
        return (int)(baseXP * Mathf.Pow(increase20perc, level));
    }

    // public int CubicLevelUpBoundary()
    // {
    //     return (int)(baseXP * Mathf.Pow(level, 3));
    // }

    public int Increase(int skill)
    {
        return skill += 1;
    }

    public void LevelUp()
    {
        // Eventually will have interactive skill point selection
        // For now just add +1 to crafting/other skills we put here for demo
        level += 1;
        crafting = Increase(crafting);
        mining = Increase(mining);

        GD.Print(
            $"\nCurrent Level: {level}\n" +
            $"Crafting: {crafting}\n" +
            $"Mining:   {mining}\n"
        );

        EmitSignal(SignalName.LevelUpNotification);
    }
}
