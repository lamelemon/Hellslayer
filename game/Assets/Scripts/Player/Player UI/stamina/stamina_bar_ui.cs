using UnityEngine;
using UnityEngine.UI;

public class stamina_bar_ui : MonoBehaviour
{
    [SerializeField] private Slider slider; // Marked as [SerializeField] to show in the Inspector

    public void stamina_bar_set(int stamina)
    {
        slider.value = stamina;
    }

    public void stamina_bar_max(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
}