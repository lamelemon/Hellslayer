using UnityEngine;
using System;


// Notes:
// - This script is attached to the player arms object and is responsible for playing sound effects based on animation events.
// - It uses the PlayerMovement script to check if the player is grounded before playing the sound effects.
// - This way you can play sound effect base of the animation frames and when player moves fast the animator speed up the animation and sound effect plays faster.
public class ArmsSoundEventFunc : MonoBehaviour
{
    [Header("Dependencies")]
    public PlayerMovement playerMovement; // Reference to the PlayerMovement script

    [Header("Settings")]
    [Range(0.01f, 20.0f)] public float SprintSoundPitchMin = 0.70f;
    [Range(0.01f, 20.0f)] public float SprintSoundPitchMax = 0.82f;

    [Range(0.01f, 20.0f)] public float FirstAttackSoundPitchMin = 0.70f;
    [Range(0.01f, 20.0f)] public float FirstAttackSoundPitchMax = 0.82f;

    [Range(0.01f, 20.0f)] public float SecondAttackSoundPitchMin = 0.75f;
    [Range(0.01f, 20.0f)] public float SecondAttackSoundPitchMax = 0.87f;

    [Range(0.01f, 20.0f)] public float ThirdAttackSoundPitchMin = 0.82f;
    [Range(0.01f, 20.0f)] public float ThirdAttackSoundPitchMax = 0.90f;

    // Sound event functions mainly used for animations that clearly have spesific frame that sound should play
    public void SprintSoundPlay() // function that plays based on animation event
    {
        if (playerMovement.IsGrounded) // Ensure the player is grounded
        {
            float SprintRandomPitch = UnityEngine.Random.Range(SprintSoundPitchMin, SprintSoundPitchMax);
            //Debug.Log("Sprint sound played with pitch: " + sprintRandomPitch); // Debug log for testing
            audio_manager.Instance.PlaySFX("PlayerSprint", SprintRandomPitch); // Play sprint sound with custom pitch
        }
    }

    public void FirstAttackSoundPlay()
    {
        float FistAttackRandomPitch = UnityEngine.Random.Range(FirstAttackSoundPitchMin, FirstAttackSoundPitchMax);
        audio_manager.Instance.PlaySFX("PlayerFistAttack", FistAttackRandomPitch);
    }
    public void SecondAttackSoundPlay() 
    {
        float FistAttackRandomPitch = UnityEngine.Random.Range(SecondAttackSoundPitchMin, SecondAttackSoundPitchMax);
        audio_manager.Instance.PlaySFX("PlayerFistAttack", FistAttackRandomPitch);
    }
    public void ThirdAttackSoundPlay()
    {
        float FistAttackRandomPitch = UnityEngine.Random.Range(ThirdAttackSoundPitchMin, ThirdAttackSoundPitchMax);
        audio_manager.Instance.PlaySFX("PlayerFistAttack", FistAttackRandomPitch);
    }
}
