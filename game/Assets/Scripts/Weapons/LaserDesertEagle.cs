using UnityEngine;
using System.Collections;

public class LaserDesertEagle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private ItemSlotHandler itemSlotHandler;

    [Header("Weapon Stats")]
    public float fireRate = 0.5f;
    public float laserDuration = 0.05f;
    public float range = 100f;
    public float damage = 25f;
    public int maxAmmo = 7;
    public float reloadTime = 3f;

    [Header("Accuracy Settings")]
    [SerializeField] private float crouchAccuracy = 0.005f;
    [SerializeField] private float hipAccuracy = 0.05f;
    [SerializeField] private float moveAccuracy = 0.08f;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading, hasFiredThisFrame;

    private Coroutine reloadCoroutine, laserCoroutine;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    void Awake() => currentAmmo = maxAmmo;

    void Start() => laserLine.enabled = false;

    void Update()
    {
        if (!enabled || isReloading) return;

        if (getInput.ReloadInput?.WasPressedThisFrame() == true && currentAmmo < maxAmmo)
        {
            if (reloadCoroutine == null) reloadCoroutine = StartCoroutine(Reload());
            return;
        }

        if (!hasFiredThisFrame && getInput.AttackInput.WasPressedThisFrame() &&
            Time.time >= nextFireTime && currentAmmo > 0)
        {
            hasFiredThisFrame = true;
            nextFireTime = Time.time + fireRate;
            Fire();
        }
    }

    void LateUpdate() => hasFiredThisFrame = false;

    void Fire()
    {
        currentAmmo--;
        itemSlotHandler?.UpdateSlotIcons();

        float spread = getInput.CrouchInput.IsPressed() ? crouchAccuracy :
                       getInput.MoveInput.IsPressed() ? moveAccuracy : hipAccuracy;

        Vector3 direction = playerCamera.transform.forward +
                            new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0f);

        Vector3 origin = playerCamera.transform.position;
        Vector3 hitPoint = origin + direction.normalized * range;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, range))
        {
            hitPoint = hit.point;
            hit.collider.GetComponentInParent<EnemyHealth>()?.TakeDamage((int)damage);
        }

        if (laserCoroutine != null) StopCoroutine(laserCoroutine);
        laserCoroutine = StartCoroutine(FireLaser(hitPoint));

        if (currentAmmo <= 0 && reloadCoroutine == null)
            reloadCoroutine = StartCoroutine(Reload());
    }

    IEnumerator FireLaser(Vector3 hitPoint)
    {
        laserLine.SetPosition(0, muzzlePoint.position);
        laserLine.SetPosition(1, hitPoint);
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
        laserCoroutine = null;
    }

    IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        itemSlotHandler?.UpdateSlotIcons();
        isReloading = false;
        reloadCoroutine = null;
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        isReloading = false;
        reloadCoroutine = null;
    }

    public void SetEquipped(bool equipped)
    {
        if (!equipped) CancelReload();
    }
}
