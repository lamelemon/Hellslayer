using UnityEngine;
using System;

public class EnemySound : MonoBehaviour
{
    [Range(0.01f, 20.0f)] public float SpikyHitSoundPitchMin = 0.80f;
    [Range(0.01f, 20.0f)] public float SpikyHitSoundPitchMax = 0.82f;
    public hp_system hp_system; // Reference to the hp_system script
    private int LastHp; // Variable to store the last HP value
    private bool playonce = false; // Variable to check if the sound has played once

    // Update is called once per frame
    void Start()
    {
        LastHp = hp_system.current_hp; // Initialize LastHp with the current HP
    }
    void Update()
    {
        if (LastHp != hp_system.current_hp)
        {
            LastHp = hp_system.current_hp; // Update LastHp with the current HP
            if (!playonce)
            {
                playonce = true; // Set playonce to true to prevent multiple sound plays
                SpikyHitSoundPlay();
            }
        }
    }
    public void SpikyHitSoundPlay()
    {
        float FistAttackRandomPitch = UnityEngine.Random.Range(SpikyHitSoundPitchMin, SpikyHitSoundPitchMax);
        audio_manager.Instance.PlaySFX("SpikyHit", FistAttackRandomPitch);
        playonce = false; // Reset playonce to allow sound to play again
    }
}
