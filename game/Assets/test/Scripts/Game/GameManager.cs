using UnityEngine;

public class GameManager : MonoBehaviour
{
    PlayerInputSubscription GetInput;

    private void Awake()
    {
        GetInput = GetComponent<PlayerInputSubscription>();   
    }
}
