using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image targetGraphic;          // The UI element whose color will change
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private void Start()
    {
        if (targetGraphic == null)
        {
            targetGraphic = GetComponent<Image>();
        }

        if (targetGraphic != null)
        {
            targetGraphic.color = normalColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetGraphic != null)
        {
            targetGraphic.color = hoverColor;
        }

        // Optional: Add hover sound or animation here
        Debug.Log("Hovered over: " + gameObject.name);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetGraphic != null)
        {
            targetGraphic.color = normalColor;
        }

        // Optional: Stop animation or sound
    }
}
