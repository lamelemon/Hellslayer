using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TextHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI text;

    public Color normalColor = Color.white;
    public Color hoverColor = new Color(5f, 5f, 0f, 1f); // Bright HDR yellow
    public Color clickColor = new Color(0f, 5f, 5f, 1f); // Bright HDR cyan

    private bool isHovered = false;

    void Start()
    {
        text.color = normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        text.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        text.color = clickColor;
    }
}
