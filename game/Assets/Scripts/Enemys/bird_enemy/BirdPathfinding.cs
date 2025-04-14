using UnityEngine;
using UnityEngine.AI;


// This script handles enemy pathfinding using Unity's NavMeshAgent.
// It assigns the player's transform as the target and updates the enemy's destination to follow the player.


public class BirdPathfinding : MonoBehaviour
{
    [Header("Dedection Settings")]
    [SerializeField] private float WatchRange = 3f;
    private NavMeshAgent navMeshAgent;
    private Transform target; // Variable to store the target's transform
    

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance; // ingore other navmesh agents
        if (player_manager.instance != null && player_manager.instance.player != null)
        {
            target = player_manager.instance.player.transform; // Assign the player's transform to the target
        }
    }

    void Update()
    {
        if (target != null)
        {
            if (IsPlayerWatchRange())
            {
                // If the player is within watch range, update the destination to the player's position
                navMeshAgent.SetDestination(target.position);
            }
            else
            {
                // If the player is out of watch range, stop the NavMeshAgent
                navMeshAgent.ResetPath();
            }
        }
    }

    private bool IsPlayerWatchRange()
    {
        float sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= WatchRange * WatchRange;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, WatchRange);
    }
}