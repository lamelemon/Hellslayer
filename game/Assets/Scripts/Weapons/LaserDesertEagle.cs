using UnityEngine;
using System.Collections;

public class LaserDesertEagle : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private AmmoTextHandler ammoTextHandler;

    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    public float laserDuration = 0.05f;
    public float range = 100f;
    public float damage = 25f;

    public int maxAmmo = 7;
    private int currentAmmo;
    public float reloadTime = 3f;
    private bool isReloading = false;

    public float crouchAccuracy = 0.005f;
    public float hipAccuracy = 0.05f;
    public float moveAccuracy = 0.08f;

    private bool isMoving;
    private bool isCrouching;
    private Coroutine reloadCoroutine;
    private Coroutine laserCoroutine;

    private bool hasFiredThisFrame = false;

    public int CurrentAmmo => currentAmmo;
    public int MaxAmmo => maxAmmo;

    void Start()
    {
        laserLine.enabled = false;
        currentAmmo = maxAmmo;
    }

    void OnEnable()
    {
        ammoTextHandler?.Show(this);
        UpdateAmmoUI();
    }

    void OnDisable()
    {
        ammoTextHandler?.Hide();
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy || isReloading)
            return;

        isMoving = getInput.MoveInput.IsPressed();
        isCrouching = getInput.CrouchInput.IsPressed();

        if (getInput.ReloadInput != null && getInput.ReloadInput.WasPressedThisFrame() && currentAmmo < maxAmmo)
        {
            if (reloadCoroutine == null)
                reloadCoroutine = StartCoroutine(Reload());
            return;
        }

        if (!hasFiredThisFrame && getInput.AttackInput.WasPressedThisFrame() && Time.time >= nextFireTime && currentAmmo > 0)
        {
            hasFiredThisFrame = true;
            nextFireTime = Time.time + fireRate;
            Fire();
        }

        UpdateAmmoUI();
    }

    void LateUpdate()
    {
        hasFiredThisFrame = false;
    }

    void Fire()
    {
        Debug.Log("Fire() called at frame: " + Time.frameCount);

        currentAmmo--;

        float spread = isCrouching ? crouchAccuracy : (isMoving ? moveAccuracy : hipAccuracy);

        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Vector3 randomOffset = new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        Vector3 spreadDirection = (ray.direction + randomOffset).normalized;
        Ray spreadRay = new Ray(ray.origin, spreadDirection);

        Vector3 hitPoint;

        if (Physics.Raycast(spreadRay, out RaycastHit hit, range))
        {
            hitPoint = hit.point;

            EnemyHealth enemy = hit.collider.GetComponentInParent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
                Debug.Log($"Hit enemy: {enemy.gameObject.name} for {damage} damage");
            }
        }
        else
        {
            hitPoint = spreadRay.GetPoint(range);
        }

        if (laserCoroutine != null)
        {
            StopCoroutine(laserCoroutine);
        }

        laserCoroutine = StartCoroutine(FireLaser(hitPoint));

        Debug.DrawRay(spreadRay.origin, spreadRay.direction * range, Color.red, 0.5f);

        if (currentAmmo <= 0 && reloadCoroutine == null)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }
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
        isReloading = false;
        reloadCoroutine = null;
        UpdateAmmoUI();
    }

    public void CancelReload()
    {
        if (reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
        isReloading = false;
    }

    public void SetEquipped(bool equipped)
    {
        if (equipped)
        {
            ammoTextHandler?.Show(this);
            UpdateAmmoUI();
        }
        else
        {
            ammoTextHandler?.Hide();
            CancelReload();
        }
    }

    void UpdateAmmoUI()
    {
        ammoTextHandler?.UpdateAmmo();
    }
}
