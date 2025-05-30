using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveUi : MonoBehaviour
{
    [Header("Wave UI Settings")]
    public string WaveTextFormat = "Next Wave in: "; // Format for displaying the wave number
    public string InWaveTextFormat = "Kill enemies to start next Wave!"; // Format for displaying the wave number
    public TextMeshProUGUI waveText; // Assign in inspector

    public EnemySpawner EnemySpawner; // Drag in the inspector
    // Update is called once per frame

    void Awake()
    {
        if (EnemySpawner == null)
            EnemySpawner = FindObjectOfType<EnemySpawner>(); // fallback if not assigned
    }

    void Update()
    {
        if (EnemySpawner.isWaveCountdownRunning)
        {
            float time = Mathf.Ceil(EnemySpawner.waveCountdownTimer);
            waveText.text = WaveTextFormat + time.ToString();
        }
        else
        {
            waveText.text = InWaveTextFormat;
        }
    }
}
