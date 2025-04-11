using UnityEngine;
using UnityEngine.AI;


// This script handles enemy pathfinding using Unity's NavMeshAgent.
// It assigns the player's transform as the target and updates the enemy's destination to follow the player.


public class EnemyPathfinding : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform target; // Variable to store the target's transform

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (player_manager.instance != null && player_manager.instance.player != null)
        {
            target = player_manager.instance.player.transform; // Assign the player's transform to the target
        }
    }

    void Update()
    {
        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }
}