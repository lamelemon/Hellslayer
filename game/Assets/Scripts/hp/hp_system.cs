using UnityEngine;
using System.Collections.Generic;
using System.Collections;


// <summary>
// This script manages the health system for a character in the game. 
// If a target is not manually assigned, it defaults to the GameObject this script is attached to.
// Drag into the collider of the object, not the object itself.


public class hp_system : MonoBehaviour
{
    public int max_hp = 100;
    public GameObject target; // reference to the main object
    public int current_hp { get; private set; } = 0;
    //public Animator anim;

    public hp_bar_ui hp_bar; // reference to hp_bar_ui script
    private LootDropping lootDropping; // Reference to the LootDropping script



    void Awake()
    {
        lootDropping = GetComponentInParent<LootDropping>();
    }
    void Start()
    {
        target = transform.root.gameObject;
        current_hp = max_hp;
        hp_bar.hp_bar_max(max_hp);
    }
    public void take_damage(int amount)  // void is a function that does not return anything
    {
        current_hp -= amount;
        hp_bar.hp_bar_set(current_hp);

        if(current_hp <= 0)
        {

            if (lootDropping != null && lootDropping.lootItems.Length > 0) // Check if loot items are available
            {
                lootDropping.DropLoot(); // Call the DropLoot method from the LootDropping script
            }
            else if (target.CompareTag("spiky")) // if target has spiky tag
            {
                return; // dont destroy/hide on this script
            }
            else
            {
                target.SetActive(false); // Deactivate the player object 
            }
        }
    }
}
