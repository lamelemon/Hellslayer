using UnityEngine;

public class VelocityApplying : MonoBehaviour
{
    [SerializeField] Vector3 objectVelocity;
    private Rigidbody rb;
    [SerializeField] bool canApplyVelocity;
    [SerializeField] bool stopVelocity;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (canApplyVelocity)
        {
            rb.linearVelocity += objectVelocity;
            canApplyVelocity = false;
        }

        if (stopVelocity)
        {
            rb.linearVelocity = Vector3.zero;
            stopVelocity = false;
        }
    }
}
