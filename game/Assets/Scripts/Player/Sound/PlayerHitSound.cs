using UnityEngine;

public class PlayerHitSound : MonoBehaviour
{
    [Range(0.01f, 20.0f)] public float PlayerHitSoundPitchMin = 0.9f;
    [Range(0.01f, 20.0f)] public float PlayerHitSoundPitchMax = 1.0f;

    public hp_system hp_system;
    private int LastHp;
    private bool firstUpdate = true;

    void Awake()
    {
        LastHp = hp_system.current_hp;
    }

    void Update()
    {
        if (firstUpdate)
        {
            LastHp = hp_system.current_hp;
            firstUpdate = false;
            return;
        }

        if (LastHp != hp_system.current_hp)
        {
            if (hp_system.current_hp > 0)
            {
                HitSoundPlay();
            }

            LastHp = hp_system.current_hp;
        }
    }

    public void HitSoundPlay()
    {
        float pitch = Random.Range(PlayerHitSoundPitchMin, PlayerHitSoundPitchMax);
        audio_manager.Instance.PlaySFX("PlayerHit", pitch);
    }
}
