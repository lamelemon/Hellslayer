using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
            hp_system hpSystem = gameObject.GetComponent<hp_system>();
            if (hpSystem.current_hp >= 0)
            {
                //hpSystem.take_damage(5);
                //Debug.Log("We hit " + enemy.name);
                gameObject.SetActive(false); // Deactivate the player object
            }
    }
}
