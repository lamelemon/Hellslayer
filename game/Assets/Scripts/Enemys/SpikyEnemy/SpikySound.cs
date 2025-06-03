using UnityEngine;

public class SpikySound : MonoBehaviour
{
    [Header("Sound Pitch Ranges")]
    [Range(0.01f, 20.0f)] public float SpikyHitSoundPitchMin = 0.80f;
    [Range(0.01f, 20.0f)] public float SpikyHitSoundPitchMax = 0.80f;
    [Range(0.01f, 20.0f)] public float SpikyDieSoundPitchMin = 0.85f;
    [Range(0.01f, 20.0f)] public float SpikyDieSoundPitchMax = 1.0f;

    [Header("References")]
    public EnemyHealth EnemyHealth;
    [HideInInspector] public PlayerScore playerScore;
    [HideInInspector] public GameObject Player; // Assign in inspector if needed

    [Header("Death Handling")]
    public GameObject keep_active_object; // e.g. health bar or particles
    public MonoBehaviour keep_active_script;
    public int RewardScoreOnDeath = 5;

    private int lastHp;
    private bool firstUpdate = true;
    private bool dieOnce = true;

    void Awake()
    {
        lastHp = EnemyHealth.currentHealth;

        if (playerScore == null)
        {
            playerScore = FindFirstObjectByType<PlayerScore>();
            if (playerScore == null)
            {
                Debug.LogWarning("PlayerScore component not found in the scene. Please assign it in the inspector or ensure it exists in the scene.");
            }
        }
    }

    void Update()
    {
        if (firstUpdate)
        {
            lastHp = EnemyHealth.currentHealth;
            firstUpdate = false;
            return;
        }

        if (lastHp != EnemyHealth.currentHealth)
        {
            if (EnemyHealth.currentHealth <= 0 && dieOnce)
            {
                dieOnce = false;
                StartCoroutine(DieSequence());
            }
            else if (EnemyHealth.currentHealth > 0)
            {
                PlayHitSound();
            }

            lastHp = EnemyHealth.currentHealth;
        }
    }

    System.Collections.IEnumerator DieSequence()
    {
        AddScoreToPlayer();

        float pitch = Random.Range(SpikyDieSoundPitchMin, SpikyDieSoundPitchMax);
        audio_manager.Instance.PlaySFX("SpikyDie", pitch);

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    public void PlayHitSound()
    {
        float pitch = Random.Range(SpikyHitSoundPitchMin, SpikyHitSoundPitchMax);
        audio_manager.Instance.PlaySFX("SpikyHit", pitch);
    }

    void DisableOtherScripts()
    {
        foreach (MonoBehaviour script in GetComponents<MonoBehaviour>())
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

    public void AddScoreToPlayer()
    {
        if (playerScore != null)
        {
            playerScore.AddScore(RewardScoreOnDeath);
        }
        else
        {
            Debug.LogWarning("playerScore is null. Cannot add score.");
        }
    }
}
