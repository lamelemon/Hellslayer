using UnityEngine;
using UnityEngine.UI;

public class combat_stamina_bar_ui : MonoBehaviour
{
    [SerializeField] private Slider slider; // Reference to the UI Slider

    public void combat_stamina_bar_set(int stamina)
    {
        slider.value = stamina;
    }

    public void combat_stamina_bar_max(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
}