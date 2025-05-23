using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefab;

    [Header("Zero Wave")]
    [SerializeField] private int waveEnemyCount = 1;
    [SerializeField] private float EnemyMinSpawnDelay = 0.5f;
    [SerializeField] private float EnemyMaxSpawnDelay = 2.0f;
    [SerializeField] private int waveEnemyCounStep = 1;
    [SerializeField] private float WaveStartDelay = 5f;
    [SerializeField] private float WaveStartDelayStepAdd = 3f;
    [SerializeField] private float SpawnRadius = 15f;

    private bool WaveCanStart = true;
    private bool shouldIncreaseDifficulty = true;
    private bool isWaitingForNextWave = false;

    private void DifficultyIncrease()
    {
        waveEnemyCount += waveEnemyCounStep; // Increase enemy count
        WaveStartDelay += WaveStartDelayStepAdd; // Increase delay between waves
    }

    void Update()
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

    private IEnumerator StartWave()
    {
        yield return new WaitForSeconds(WaveStartDelay);
        WaveCanStart = true;
        shouldIncreaseDifficulty = true;
    }

    private void SpawnEnemy(Vector3 position)
    {
        Instantiate(EnemyPrefab, position, Quaternion.identity);
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < waveEnemyCount; i++)
        {
            SpawnEnemy(GetRandomSpawnPosition());
            yield return new WaitForSeconds(GetRandomSpawnDelay());
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
