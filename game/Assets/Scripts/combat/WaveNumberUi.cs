using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveNumberUi : MonoBehaviour
{
    [Header("Wave UI Settings")]
    public string WaveTextFormat = "Wave: "; // Format for displaying the wave number
    public TextMeshProUGUI waveText; // Assign in inspector

    public EnemySpawner EnemySpawner; // Drag in the inspector
    public int wave = 0;
    // Update is called once per frame

    void Awake()
    {
        if (EnemySpawner == null)
            EnemySpawner = FindFirstObjectByType<EnemySpawner>(); // fallback if not assigned
    }

    void Update()
    {
        if (EnemySpawner.currentWave != wave)
        {
            wave = EnemySpawner.currentWave;
            waveText.text = WaveTextFormat + EnemySpawner.currentWave.ToString();
        }
    }
}
