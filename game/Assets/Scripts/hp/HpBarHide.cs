using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HpBarHide : MonoBehaviour
{
    private EnemyHealth EnemyHealth;
    public UnityEngine.UI.Image Fill;
    public UnityEngine.UI.Image Border;

    void Start()
    {
        EnemyHealth = GetComponent<EnemyHealth>();
    }

    void Update()
    {
        if (EnemyHealth != null)
        {
            if (EnemyHealth.maxHealth != EnemyHealth.currentHealth)
            {
                Fill.enabled = true;
                Border.enabled = true;
                this.enabled = false; // Disable this script to stop checking
            }
        }
    }
}
