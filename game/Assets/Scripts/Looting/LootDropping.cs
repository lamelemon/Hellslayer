using System;
using System.Collections;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

// HOX!!!!!!!!!!!!! >:( 
// Make the droppable items into drops by dragging them into the lootItems array in the inspector

public class LootDropping : MonoBehaviour
{
    public LootItem[] lootItems; // Array to hold loot items
    public void DropLoot()
    {
        // Check if lootItems is not null and has items
        if (lootItems.Length > 0)
        {
            GameObject[] droppedItems = LootItem.InstantiateLootItems(LootItem.FindItemByRandomWeight(lootItems), transform.position);
            Collider[][] colliders = LootItem.GetColliders(droppedItems);
            Rigidbody[] rb = LootItem.GetRigidbodies(droppedItems);
            foreach (Rigidbody rigidbody in rb)
            {
                rigidbody.isKinematic = true; // Set all rigidbodies to kinematic
            }
        
            //StartCoroutine(DelayedStuff(lootItem, rb, colliders));
        }
    }

    private float GetHeight(Collider[][] colliders)
    {
        foreach (Collider[] colliderArray in colliders)
        {
            foreach (Collider collider in colliderArray)
            {
                if (collider.bounds.size.y > 0)
                {
                    return collider.bounds.size.y;
                }
            }
        }

        return 0f; // Return zero if no collider found
    }
/*    IEnumerator DelayedStuff(GameObject[] obj, Rigidbody[] rb, Collider[][] colliders)
    {
        yield return new WaitForFixedUpdate();
        obj.transform.position = transform.position + GetHeight(colliders) / 2 * Vector3.up; // Adjust the position to be above the enemy
        rb.isKinematic = false; // Ensure the loot item is not kinematic
        rb.AddForce(new(UnityEngine.Random.Range(2f, 5f), 5f, UnityEngine.Random.Range(2f, 5f)), ForceMode.Impulse); // Add upward force to the loot item
        lootItems = null;
        transform.gameObject.SetActive(false); // Deactivate the enemy object
    }*/
}

[Serializable]
public class LootItem
{
    [Tooltip("The prefab of the loot item. This should be a prefab that can be instantiated in the game world.")]
    public GameObject itemPrefab;
    [Tooltip("The weight of the item. Higher weight means more likely to drop.")]
    [Range(0, 100000)]
    public int weight;
    
    [Tooltip("The range of the amount of items that can drop (min inclusive, max inclusive). PLEASE DON'T SET TO AN INSANELY HIGH NUMBER.")]
    [Range(1, 100)]
    public Vector2Int dropAmountRange;

    public int GetRandomDropAmount()
    {
        return UnityEngine.Random.Range(dropAmountRange.x, dropAmountRange.y + 1);
    }

    public static int GetTotalWeight(LootItem[] lootItems)
    {
        int totalWeight = 0;

        foreach (LootItem item in lootItems)
        {
            if (item.weight <= 0)
            {
                item.weight = 1; // Ensure weight is at least 1
            }

            totalWeight += item.weight;
        }

        return totalWeight;
    }

    public static LootItem FindItemByWeight(LootItem[] lootItems, int weight)
    {
        foreach (LootItem item in lootItems)
        {
            if (weight < item.weight)
            {
                return item;
            }
            weight -= item.weight;
        }
        return null; // Return null if no item found
    }
    public static LootItem FindItemByRandomWeight(LootItem[] lootItems)
    {
        int randomWeight = UnityEngine.Random.Range(0, GetTotalWeight(lootItems));
        return FindItemByWeight(lootItems, randomWeight);
    }

    public static float WeightToChance(int weight, int totalWeight) // use for displaying drop chance in whatever (ui or just for debugging)
    {
        return (float)weight / totalWeight;
    }

    public static GameObject InstantiateLootItem(LootItem lootItem, Vector3 position)
    {
        return GameObject.Instantiate(lootItem.itemPrefab, position, Quaternion.identity);
    }

    public static GameObject[] InstantiateLootItems(LootItem lootItem, Vector3 position)
    {
        GameObject[] lootItems = new GameObject[lootItem.GetRandomDropAmount()];
        for (int i = 0; i < lootItems.Length; i++)
        {
            lootItems[i] = InstantiateLootItem(lootItem, position);
        }
        return lootItems;
    }

    public static Collider[][] GetColliders(GameObject[] lootItems)
    {
        Collider[][] colliders = new Collider[lootItems.Length][];
        for (int i = 0; i < lootItems.Length; i++)
        {
            colliders[i] = lootItems[i].GetComponents<Collider>();
        }
        return colliders;
    }

    public static Rigidbody[] GetRigidbodies(GameObject[] lootItems)
    {
        Rigidbody[] rigidbodies = new Rigidbody[lootItems.Length];
        for (int i = 0; i < lootItems.Length; i++)
        {
            rigidbodies[i] = lootItems[i].GetComponent<Rigidbody>();
        }
        return rigidbodies;
    }
}
