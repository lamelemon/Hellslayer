using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class BasicPathfinding : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float WatchRange = 30f;
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
        if (target != null && IsPlayerWatchRange())
        {
            // If the player is within watch range and not blocked by obstacles, update the destination to the player's position
            navMeshAgent.SetDestination(target.position);
        }
        else
        {
            // If the player is out of watch range, stop the NavMeshAgent
            navMeshAgent.ResetPath();
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
