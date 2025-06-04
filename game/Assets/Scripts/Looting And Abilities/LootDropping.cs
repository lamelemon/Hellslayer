using System;
using System.Collections;
using System.Collections.Generic;
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
        if (lootItems != null && lootItems.Length > 0)
        {
            GameObject[] droppedItems = LootItem.GetLoot(lootItems, transform.position); // Get the loot items
            foreach (GameObject item in droppedItems)
            {
                item.GetComponent<Rigidbody>().isKinematic = true; // Set all rigidbodies to kinematic
            }
        
            StartCoroutine(DelayedStuff(droppedItems));
        }
    }

    private float GetHeight(Collider[] colliders)
    {
        float biggestSize = 0f;
        foreach (Collider collider in colliders)
        {
            if (collider.bounds.size.y > biggestSize)
            {
                biggestSize = collider.bounds.size.y;
            }
        }

        return biggestSize; // Return zero if no collider found
    }
    IEnumerator DelayedStuff(GameObject[] obj)
    {
        yield return new WaitForFixedUpdate();

        foreach (GameObject item in obj)
        {
            Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();
            itemRigidbody.AddForce(new(UnityEngine.Random.Range(2f, 5f), 5f, UnityEngine.Random.Range(2f, 5f)), ForceMode.Impulse);
            item.transform.position = transform.position + GetHeight(item.transform.GetComponents<Collider>()) / 2 * Vector3.up; // Adjust the position to be above the enemy
            itemRigidbody.isKinematic = false; // Set all rigidbodies to non-kinematic
        }

        lootItems = null;
        transform.gameObject.SetActive(false); // Deactivate the enemy object
    }
}

[Serializable]
public class LootItem
{
    [Tooltip("The prefab of the loot item. This should be a prefab that can be instantiated in the game world.")]
    public GameObject itemPrefab;
    [Tooltip("The weight of the item. Higher weight means more likely to drop.")]
    [Range(0, 100000)]
    public int weight = 1;
    [Tooltip("The range of the amount of items that can drop (min inclusive, max inclusive). PLEASE DON'T SET TO AN INSANELY HIGH NUMBER.")]
    public Vector2Int dropAmountRange = new(1, 1); // Range of items that can drop
    public bool isStackable = true; // Whether the item is stackable or not
    public bool isGuaranteed = false; // Whether the item is guaranteed to drop or not

    public int GetRandomDropAmount()
    {
        return UnityEngine.Random.Range(Math.Min(dropAmountRange.x, dropAmountRange.y), Math.Max(dropAmountRange.x, dropAmountRange.y) + 1);
    }

    public static int GetTotalWeight(LootItem[] lootItems)
    {
        int totalWeight = 0;

        foreach (LootItem item in lootItems)
        {
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

    public static GameObject[] InstantiateLootItems(List<LootItem> drop, Vector3 position)
    {
        List<GameObject> lootItems = new();

        foreach (LootItem item in drop)
        {
            int amount = item.GetRandomDropAmount();
            for (int i = 0; i < amount; i++)
            {
                lootItems.Add(InstantiateLootItem(item.itemPrefab, position));
            }
        }

        return lootItems.ToArray();
    }

    public static GameObject InstantiateLootItem(GameObject item, Vector3 position)
    {
        return GameObject.Instantiate(item, position, Quaternion.identity);
    }

    public static LootItem[] GetGuaranteedLoot(LootItem[] lootItems)
    {
        List<LootItem> guaranteedLoot = new();

        foreach (LootItem item in lootItems)
        {
            if (item.isGuaranteed)
            {
                guaranteedLoot.Add(item);
            }
        }
        return guaranteedLoot.ToArray();
    }

    public static GameObject[] GetLoot(LootItem[] lootItems, Vector3 position)
    {
        List<LootItem> loot = new();

        loot.AddRange(GetGuaranteedLoot(lootItems));
        loot.Add(FindItemByRandomWeight(lootItems));

        return InstantiateLootItems(loot, position);
    }
}
