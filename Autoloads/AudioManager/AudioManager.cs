using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node
{
    [Export] public int MaxSimultaneousSfx = 8;

    private AudioStreamPlayer _musicPlayer;
    private List<AudioStreamPlayer> _sfxPool = new();

    public override void _Ready()
    {
        _musicPlayer = GetNode<AudioStreamPlayer>("MusicPlayer");

        // Collect SFX players by naming convention
        for (int i = 0; i < MaxSimultaneousSfx; i++)
        {
            var player = GetNode<AudioStreamPlayer>($"SFXPlayers/SFXStreamPlayer{i}");
            if (player != null)
            {
                _sfxPool.Add(player);
            }
        }

        if (_sfxPool.Count == 0)
        {
            GD.PushWarning("AudioManager: No SFX players found!");
        }
    }

    public void PlaySfx(AudioStream stream)
    {
        if (stream == null) return;

        foreach (var player in _sfxPool)
        {
            if (!player.Playing)
            {
                player.Stream = stream;
                player.PitchScale = (float)GD.RandRange(0.95, 1.05);
                player.Play();
                return;
            }
        }

        GD.PushWarning("AudioManager: All SFX players busy. Sound skipped.");
    }

    public void PlayMusic(AudioStream stream)
    {
        if (stream == null) return;

        _musicPlayer.Stream = stream;
        _musicPlayer.Play();
    }

    public void StopMusic()
    {
        _musicPlayer.Stop();
    }
}
