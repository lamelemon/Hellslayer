using UnityEngine;

public class SpikySound : MonoBehaviour
{
    [Range(0.01f, 20.0f)] public float SpikyHitSoundPitchMin = 0.80f;
    [Range(0.01f, 20.0f)] public float SpikyHitSoundPitchMax = 0.82f;
    [Range(0.01f, 20.0f)] public float SpikyDieSoundPitchMin = 0.85f;
    [Range(0.01f, 20.0f)] public float SpikyDieSoundPitchMax = 1.0f;

    public hp_system hp_system;
    private int LastHp;
    private bool firstUpdate = true;
    private bool DieOnce = true;

    public GameObject keep_active_object; // e.g. health bar or particles
    public MonoBehaviour keep_active_script;

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
            if (hp_system.current_hp <= 0 && DieOnce)
            {
                DieOnce = false;
                StartCoroutine(DieSequence());
            }
            else if (hp_system.current_hp > 0)
            {
                SpikyHitSoundPlay();
            }

            LastHp = hp_system.current_hp;
        }
    }

    System.Collections.IEnumerator DieSequence()
    {
        // Step 2: Disable all children
        DisableAllChildren(gameObject);

        // Step 3: Disable other scripts on this GameObject
        DisableOtherScripts();

        // Step 4: Play death sound
        float pitch = Random.Range(SpikyDieSoundPitchMin, SpikyDieSoundPitchMax);
        audio_manager.Instance.PlaySFX("SpikyDie", pitch);

        // Step 5: Wait before destroying
        yield return new WaitForSeconds(0.5f); // adjust if sound is longer

        // Step 6: Destroy self
        Destroy(gameObject);
    }

    public void SpikyHitSoundPlay()
    {
        float pitch = Random.Range(SpikyHitSoundPitchMin, SpikyHitSoundPitchMax);
        audio_manager.Instance.PlaySFX("SpikyHit", pitch);
    }

    void DisableOtherScripts()
    {
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script != keep_active_script)
            {
                script.enabled = false;
            }
        }
    }

    void DisableAllChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            if (keep_active_object != null && child.gameObject == keep_active_object)
                continue;

            child.gameObject.SetActive(false);
        }
    }
}
