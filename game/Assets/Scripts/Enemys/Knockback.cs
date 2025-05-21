using UnityEngine;

/// summary
/// !!! Use this mainly only for enemy intereaction with player, other enemys not for players combat use PlayerCombat.cs
/// Handles knockback mechanics for objects with specific tags or all rigidbodies.
public class Knockback : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private Collider knockbackCollider;
    [SerializeField] private float defaultKnockbackForce = 10f;
    [SerializeField] private bool canKnockbackAllRbs = false;

    [Header("Target Settings")]
    [SerializeField] private string targetTag = "";
    [SerializeField] private float targetTagKnockbackForce = 10f;

    [Header("Secondary Target Settings")]
    [SerializeField] private string secondTargetTag = "";
    [SerializeField] private float secondTargetTagKnockbackForce = 10f;

    private void OnCollisionStay(Collision collision)
    {
        float knockbackForce = GetKnockbackForce(collision.gameObject);
        if (knockbackForce <= 0) return;

        ApplyKnockback(collision, knockbackForce);
    }

    private float GetKnockbackForce(GameObject target)
    {
        if (!string.IsNullOrEmpty(targetTag) && target.CompareTag(targetTag))
            return targetTagKnockbackForce;

        if (!string.IsNullOrEmpty(secondTargetTag) && target.CompareTag(secondTargetTag))
            return secondTargetTagKnockbackForce;

        return canKnockbackAllRbs ? defaultKnockbackForce : 0f;
    }

    private void ApplyKnockback(Collision collision, float force)
    {
        Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (targetRigidbody == null) return;

        Vector3 knockbackDirection = (collision.transform.position - transform.position).normalized;
        targetRigidbody.AddForce(knockbackDirection * force, ForceMode.Impulse);
    }
}
