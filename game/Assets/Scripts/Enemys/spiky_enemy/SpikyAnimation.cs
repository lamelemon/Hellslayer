using UnityEngine;
using UnityEngine.AI;

public class SpikyAnimation : MonoBehaviour
{
    private Animator animator; // Reference to the animator
    private NavMeshAgent agent; // Reference to the NavMeshAgent

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float currentSpeed = agent.velocity.magnitude;
        float maxSpeed = agent.speed;
        float normalizedSpeed = Mathf.InverseLerp(0f, maxSpeed, currentSpeed);

        animator.SetFloat("speed", normalizedSpeed);
        //Debug.Log(normalizedSpeed);
    }
}
