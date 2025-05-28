using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class HpBarHide : MonoBehaviour
{
    private hp_system hpSystem;
    public UnityEngine.UI.Image Fill;
    public UnityEngine.UI.Image Border;

    void Start()
    {
        hpSystem = GetComponent<hp_system>();
    }

    void Update()
    {
        if (hpSystem != null)
        {
            if (hpSystem.max_hp != hpSystem.current_hp)
            {
                Fill.enabled = true;
                Border.enabled = true;
                this.enabled = false; // Disable this script to stop checking
            }
        }
    }
}
