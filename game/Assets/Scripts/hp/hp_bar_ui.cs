using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class hp_bar_ui : MonoBehaviour
{
    public Slider slider;
    public void hp_bar_set(int hp)
    {
        slider.value = hp;
    }
    public void hp_bar_max(int hp)  
    {
        slider.maxValue = hp;
        slider.value = hp;
    }
}