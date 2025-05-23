using UnityEngine;
using UnityEngine.UI;

public class WeaponHeatUI : MonoBehaviour
{
    [SerializeField] private Slider heatSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Color normalColor = Color.yellow;
    [SerializeField] private Color overheatedColor = Color.red;

    private float currentHeat = 0f;
    private float maxHeat = 100f;
    private float cooldownRate = 15f;
    private float overheatCooldown = 2f;
    private float overheatTimer = 0f;
    private bool isOverheated = false;
    private bool isVisible = false;

    private void Start()
    {
        SetVisible(false);
    }

    private void Update()
    {
        if (isOverheated)
        {
            overheatTimer -= Time.deltaTime;
            if (overheatTimer <= 0f)
            {
                isOverheated = false;
                currentHeat = 0f;       // Instantly reset heat
                UpdateUI();
            }
        }
        else if (currentHeat > 0f)
        {
            currentHeat -= cooldownRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
            UpdateUI();
        }

        // Hide bar only if fully cooled and not overheated
        if (currentHeat <= 0f && !isOverheated && isVisible)
        {
            SetVisible(false);
        }
    }

    public void AddHeat(float amount, float max, float cooldown, float overheatDelay)
    {
        if (isOverheated) return;

        maxHeat = max;
        cooldownRate = cooldown;
        overheatCooldown = overheatDelay;

        currentHeat += amount;
        if (currentHeat >= maxHeat)
        {
            currentHeat = maxHeat;
            isOverheated = true;
            overheatTimer = overheatCooldown;
        }

        SetVisible(true);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (heatSlider != null)
            heatSlider.value = currentHeat / maxHeat;

        if (fillImage != null)
            fillImage.color = isOverheated ? overheatedColor : normalColor;
    }

    private void SetVisible(bool visible)
    {
        isVisible = visible;
        if (heatSlider != null)
            heatSlider.gameObject.SetActive(visible);
    }

    public bool IsOverheated()
    {
        return isOverheated;
    }
}
