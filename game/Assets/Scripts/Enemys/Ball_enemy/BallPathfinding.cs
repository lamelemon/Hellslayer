using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class BallPathfinding : MonoBehaviour
{
    private Transform target;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private float WatchRange = 3f;

    void Awake()
    {
        target = player_manager.instance.player.transform;;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (IsPlayerWatchRange())
        {
            navMeshAgent.SetDestination(target.position);
        }
        else
        {
            navMeshAgent.ResetPath();
        }
    }
    private bool IsPlayerWatchRange()
    {
        float sqrDistance = (target.position - transform.position).sqrMagnitude;
        return sqrDistance <= WatchRange * WatchRange;
    }
}
