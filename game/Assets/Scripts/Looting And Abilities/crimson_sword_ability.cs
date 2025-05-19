using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrimsonSwordAbility : MonoBehaviour
{
    [SerializeField] private PlayerInputManager GetInput;
    [SerializeField] private float SpecialRayLength = 35f; // Length of the spherecast
    [SerializeField] private float sphereRadius = 3f; // Radius of the spherecast
    [SerializeField] private Camera PlayerCamera;
    [SerializeField] private int damageValue = 12; // Damage to deal
    [SerializeField] private LineRenderer beamVisual;
    [SerializeField] private PlayerInteraction PlayerItemInteraction;

    private Vector3 storedRayOrigin;
    private Vector3 storedRayDirection;
    private bool isRayActive = false;
    private float lastUsedTime = -Mathf.Infinity;
    private float cooldownDuration = 12f;
    private Coroutine damageCoroutine;

    void Update()
    {
        if (GetInput.SpecialInput.WasPressedThisFrame() && Time.time >= lastUsedTime + cooldownDuration)
        {
            if (PlayerItemInteraction.currentlyHeldItem.itemName == "CrimsonSword")
            {
                Debug.Log("Special ability activated!");

                Ray ray = PlayerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                if (Physics.SphereCast(ray, sphereRadius, out RaycastHit hitInfo, SpecialRayLength))
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
        }
        else if (GetInput.SpecialInput.WasPressedThisFrame())
        {
            Debug.Log("Ability is on cooldown!");
        }
    }

    void OnDrawGizmos()
    {
        if (isRayActive)
        {
            Gizmos.color = Color.red;

            // Draw sphere trail along path
            int segments = 10;
            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                Vector3 point = storedRayOrigin + storedRayDirection * (SpecialRayLength * t);
                Gizmos.DrawWireSphere(point, sphereRadius);
            }
        }
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
            Ray ray = new Ray(storedRayOrigin, storedRayDirection);
            RaycastHit[] hits = Physics.SphereCastAll(ray, sphereRadius, SpecialRayLength);

            HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.GetComponentInParent<EnemyHealth>() is EnemyHealth enemyHealth &&
                    damagedEnemies.Add(enemyHealth))
                {
                    enemyHealth.TakeDamage(damageValue);
                    Debug.Log($"Dealt {damageValue} damage to {hit.collider.gameObject.name}");
                }
            }

            yield return new WaitForSeconds(0.5f); // Damage every 0.5 seconds
        }
    }
}
