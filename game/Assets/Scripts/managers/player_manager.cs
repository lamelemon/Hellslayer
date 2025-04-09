using UnityEngine;
// Use this script to manage the player GameObject in the scene. This script is a Singleton, meaning there will only be one instance of it in the game.
// Stores a reference to the player GameObject, which can be assigned in the Unity Inspector.
// Other scripts can now access the player without needing a direct reference.
public class player_manager : MonoBehaviour
{
    // Creates a global reference that other scripts can access.
    #region singleton
    
    public static player_manager instance;

    #endregion

    //
    void Awake()
    {
        instance = this; // Assign Singleton Instance
    }

    public GameObject player; // Player Reference Storage
}
