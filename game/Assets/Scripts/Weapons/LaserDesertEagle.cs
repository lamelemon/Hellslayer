using UnityEngine;
using System.Collections;

public class LaserDesertEagle : MonoBehaviour
{
    // References
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private PlayerInputManager getInput;
    [SerializeField] private PlayerInteraction PlayerItemInteraction;

    // Weapon Settings
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    public float laserDuration = 0.05f;
    public float range = 100f;
    public float damage = 25f;

    // Heat System
    public float heatPerShot = 25f;
    public float maxHeat = 100f;
    public float heatCooldownRate = 15f; // per second
    public float overheatCooldownTime = 2f;

    private float currentHeat = 0f;
    private bool isOverheated = false;

    // Accuracy Settings
    public float crouchAccuracy = 0.005f;
    public float hipAccuracy = 0.05f;
    public float moveAccuracy = 0.08f;

    private bool isMoving;
    private bool isCrouching;

    void Start()
    {
        laserLine.enabled = false;
        currentHeat = 0f;
    }

    void Update()
    {
        CheckMovementInput();
        isCrouching = getInput.CrouchInput.IsPressed();

        HandleHeatCooldown();

        if (getInput.AttackInput.WasPressedThisFrame() && Time.time >= nextFireTime && !isOverheated && PlayerItemInteraction.currentlyHeldItem.itemName == "LaserDessu")
        {
            nextFireTime = Time.time + fireRate;
            Fire();
        }
    }

    void CheckMovementInput()
    {
        isMoving = getInput.MoveInput.IsPressed();
    }

    void Fire()
    {
        // Add heat
        currentHeat += heatPerShot;
        if (currentHeat >= maxHeat)
        {
            currentHeat = maxHeat;
            isOverheated = true;
            StartCoroutine(OverheatCooldown());
        }

        // Determine spread based on state
        float spread = isCrouching ? crouchAccuracy : (isMoving ? moveAccuracy : hipAccuracy);

        // Create ray from screen center
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Vector3 direction = ray.direction + new Vector3(Random.Range(-spread, spread), Random.Range(-spread, spread), 0);
        Ray spreadRay = new Ray(ray.origin, direction);

        Vector3 hitPoint;

        if (Physics.Raycast(spreadRay, out RaycastHit hit, range))
        {
            hitPoint = hit.point;
            DealDamage(hit);
        }
        else
        {
            hitPoint = spreadRay.GetPoint(range);
        }

        StartCoroutine(FireLaser(hitPoint));

        Debug.DrawRay(spreadRay.origin, spreadRay.direction * range, Color.red, 0.5f);
    }

    void DealDamage(RaycastHit hit)
    {
        if (hit.collider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemyHealth)
        {
            enemyHealth.TakeDamage((int)damage);  // <-- Cast float to int here
            Debug.Log($"{gameObject.name} attacked {hit.collider.gameObject.name} for {(int)damage} damage.");
        }
        else
        {
            Debug.LogError("EnemyHealth component is missing on the target.");
        }
    }

    IEnumerator FireLaser(Vector3 hitPoint)
    {
        laserLine.SetPosition(0, muzzlePoint.position);
        laserLine.SetPosition(1, hitPoint);
        laserLine.enabled = true;
        yield return new WaitForSeconds(laserDuration);
        laserLine.enabled = false;
    }

    void HandleHeatCooldown()
    {
        if (!isOverheated && currentHeat > 0f)
        {
            currentHeat -= heatCooldownRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
        }
    }

    IEnumerator OverheatCooldown()
    {
        yield return new WaitForSeconds(overheatCooldownTime);
        isOverheated = false;
        currentHeat = 0f;
    }
}
