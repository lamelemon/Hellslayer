using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrimsonSword : MonoBehaviour, IWeapon, ISpecialAbility
{
    public string itemName = "CrimsonSword"; // Name of the item
    public int itemValue = 10;           // Value of the item
    public int Damage { get; set; } = 30;         // Damage the item can deal
    public float AttackRange { get; set; } = 9f;      // Range of the melee attack
    public float AttackCooldown { get; set; } = 1f; // Time between attacks
    public float SpecialCooldown { get; set; } = 12f; // Cooldown time for the special ability
    public LayerMask enemyLayer;         // Layer for enemies
    [SerializeField] private LineRenderer beamVisual;
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private float SpecialWidth = 3f; // Radius of the spherecast
    [SerializeField] private float SpecialRayLength = 35f; // Length of the spherecast
    [SerializeField] private int SpecialDamage = 12; // Damage to deal with the special ability
    private Vector3 storedRayOrigin;
    private Vector3 storedRayDirection;
    private bool isRayActive = false;
    private float lastUsedTime = -Mathf.Infinity;
    private Coroutine damageCoroutine;

    public Quaternion ItemRotation = Quaternion.Euler(0, 0, 0); // Rotation of the item
    public Vector3 ItemPosition = new(0, 0, 0); // Position of the item

    [SerializeField] Transform handPosition; // Reference to the hand position where the item will be attached

    private Camera playerCamera; // Reference to the player's camera

    private void Awake()
    {
        enemyLayer = LayerMask.GetMask("enemyLayer");
        playerCamera = GameObject.FindWithTag("playerCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        // Find the camera tagged as "playerCamera"
        if (playerCamera == null)
        {
            Debug.LogError("Camera with tag 'playerCamera' not found. Please ensure your camera is tagged correctly.");
        }
    }

    public void Attack()
    {
        // Perform a raycast from the center of the screen
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, AttackRange, enemyLayer))
        {
            // Check if the hit object has an EnemyHealth component
            if (hit.collider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemyHealth)
            {
                enemyHealth.TakeDamage(Damage); // Deal damage to the enemy
                Debug.Log($"{itemName} attacked {hit.collider.gameObject.name} for {Damage} damage.");
            }
            else
            {
                Debug.LogError("EnemyHealth component is missing on the target.");
            }
        }
        else
        {
            Debug.Log("No enemy in range to attack.");
        }
    }

    public void SpecialAbility()
    {
        Debug.Log("Special ability activated!");

        Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.SphereCast(ray, SpecialWidth, out RaycastHit hitInfo, SpecialRayLength))
        {
            Debug.Log($"First hit object: {hitInfo.collider.gameObject.name}");
        }

        storedRayOrigin = ray.origin;
        storedRayDirection = ray.direction;
        isRayActive = true;

        damageCoroutine = StartCoroutine(DamageWhileActive());
        StartCoroutine(DeactivateRayAfterDelay(3f));
        lastUsedTime = Time.time;

        // Set the beam visual line positions
        beamVisual.enabled = true;
        beamVisual.SetPosition(0, storedRayOrigin);
        beamVisual.SetPosition(1, storedRayOrigin + storedRayDirection * SpecialRayLength);
    }

    private IEnumerator DeactivateRayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isRayActive = false;
        beamVisual.enabled = false;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }
    private IEnumerator DamageWhileActive()
    {
        while (isRayActive)
        {
            Ray ray = new(storedRayOrigin, storedRayDirection);
            RaycastHit[] hits = Physics.SphereCastAll(ray, SpecialWidth, SpecialRayLength);

            HashSet<EnemyHealth> damagedEnemies = new();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemyHealth && damagedEnemies.Add(enemyHealth))
                {
                    enemyHealth.TakeDamage(SpecialDamage);
                    Debug.Log($"Dealt {SpecialDamage} damage to {hit.collider.gameObject.name}");
                }
            }

            yield return new WaitForSeconds(0.5f); // Damage every 0.5 seconds
        }
    }
}
