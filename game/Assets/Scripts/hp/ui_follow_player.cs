using UnityEngine;

public class ui_follow_player : MonoBehaviour
{
    private Transform player;
    void Start()
    {
        player = player_manager.instance.player.transform;
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set. Please assign a player Transform.");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPosition = player.position;
        targetPosition.y = transform.position.y; // Keep the ui bar upright by aligning only the X and Z axes
        transform.LookAt(targetPosition);
    }
}
