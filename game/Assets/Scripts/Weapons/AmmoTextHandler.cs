using UnityEngine;
using TMPro;

public class AmmoTextHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text ammoText;
    private LaserDesertEagle currentWeapon;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ammoText == null)
            ammoText = GetComponent<TMP_Text>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(LaserDesertEagle weapon)
    {
        currentWeapon = weapon;
        gameObject.SetActive(true);
        UpdateAmmo();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (currentWeapon != null)
        {
            currentWeapon.CancelReload();
            currentWeapon = null;
        }
    }

    public void UpdateAmmo()
    {
        if (currentWeapon != null && ammoText != null)
            ammoText.text = $"{currentWeapon.CurrentAmmo} / {currentWeapon.MaxAmmo}";
    }
}
