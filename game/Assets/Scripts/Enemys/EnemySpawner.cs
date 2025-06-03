using UnityEngine;
using System.Collections;
// <summary>
// EnemySpawner is responsible for spawning enemies in waves with increasing difficulty.

// !!! need make more wawe like you can use player score to start wave
public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public GameObject SecondEnemyPrefab;
    public GameObject BossEnemyPrefab; // Optional: Add a boss enemy prefab if needed
    public GameObject[] EnemyPrefabs; // Array to hold different enemy prefabs

    [Header("Wave settings")]
    [SerializeField] private int waveEnemyCount = 1;
    [SerializeField] private float EnemyMinSpawnDelay = 0.1f;
    [SerializeField] private float EnemyMaxSpawnDelay = 0.3f;
    [SerializeField] private int waveEnemyCounStepAdd = 1;
    [SerializeField] private float WaveStartDelay = 5f;
    [SerializeField] private float SpawnRadius = 30f;
    [Header("Wave Difficulty Settings")]
    [SerializeField] private int BabyWaveCount = 15;
    [SerializeField] private float WaveStartDelayStepAdd = 6f; // Delay between waves increases by this amount each time
    [SerializeField] private float WaveStartDelayStepSub = 1f;

    [Header("References")]
    [SerializeField] private PlayerScore PlayerScore;

    [HideInInspector] public int currentWave = 0;
    private int CurrentPlayerScore = 0; // reward score for spiky is 5 and Bird is 10

    [HideInInspector] public bool WaveCanStart = true;
    private bool shouldIncreaseDifficulty = true;
    private bool isWaitingForNextWave = false;

    public float waveCountdownTimer { get; private set; } = 0f;
    public bool isWaveCountdownRunning { get; private set; } = false;
    private void DifficultyIncrease()
    {
        if (currentWave < BabyWaveCount)
        {
            waveEnemyCount += waveEnemyCounStepAdd; // Increase enemy count
            WaveStartDelay += WaveStartDelayStepAdd; // Increase delay between waves
        }
        else
        {
            waveEnemyCount += waveEnemyCounStepAdd + waveEnemyCounStepAdd; // Increase enemy count by 2x
            WaveStartDelay -= WaveStartDelayStepSub; // Decrease delay between waves
        }
    }

    void Update()
    {
        CurrentPlayerScore = PlayerScore.CurrentScore;
        //Debug.Log($"Current all Player Score: {PlayerScore.SumScore}");
        /*Debug.Log($"Current Player Score: {CurrentPlayerScore}");
        Debug.Log($"Current Wave: {currentWave}");
        Debug.Log($"Needed scpre to start wave: {(waveEnemyCount -1) * 5}");*/
        if (CurrentPlayerScore >= 5 * (waveEnemyCount -1)) // has minimal score to start wave. Like start wave if has minimal rewards score from enemies
        {
            if (WaveCanStart)
            {
                WaveCanStart = false;
                StartCoroutine(SpawnWave());
                isWaitingForNextWave = false;
            }
            else if (!isWaitingForNextWave)
            {
                StartCoroutine(StartWave());
                isWaitingForNextWave = true;
            }
        }
    }

    private IEnumerator StartWave()
    {
        waveCountdownTimer = WaveStartDelay;
        isWaveCountdownRunning = true;

        while (waveCountdownTimer > 0f)
        {
            waveCountdownTimer -= Time.deltaTime;
            yield return null;
        }

        waveCountdownTimer = 0f;
        isWaveCountdownRunning = false;

        WaveCanStart = true;
        shouldIncreaseDifficulty = true;
    }

    private void SpawnEnemy(Vector3 position)
    {
        Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)], position, Quaternion.identity);
    }

    private void PickRandomEnemyPrefab()
    {
        if (EnemyPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, EnemyPrefabs.Length);
            EnemyPrefab = EnemyPrefabs[randomIndex];
        }
        else
        {
            Debug.LogWarning("No enemy prefabs assigned in the EnemySpawner.");
        }
    }

    private IEnumerator SpawnWave()
    {
        currentWave += 1;
        PlayerScore.ResetScore();

        if (currentWave == 10) Instantiate(BossEnemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        else
        {
            for (int i = 0; i < waveEnemyCount; i++)
            {
                SpawnEnemy(GetRandomSpawnPosition());
                yield return new WaitForSeconds(GetRandomSpawnDelay());
            }
        }
        // Increase difficulty after the wave ends
        if (shouldIncreaseDifficulty)
        {
            DifficultyIncrease();
            shouldIncreaseDifficulty = false;
        }
    }

    private float GetRandomSpawnDelay()
    {
        return Random.Range(EnemyMinSpawnDelay, EnemyMaxSpawnDelay);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float radius = Random.Range(0f, SpawnRadius);
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        return transform.position + offset;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, SpawnRadius);
    }
}
