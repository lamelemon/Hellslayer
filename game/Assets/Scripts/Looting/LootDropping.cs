using System;
using System.Collections;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;

// HOX!!!!!!!!!!!!! >:( 
// Make the droppable items into drops by dragging them into the prefabs folder and then drag them into the lootItems array in the inspector

public class LootDropping : MonoBehaviour
{
    public LootItem[] lootItems; // Array to hold loot items
    public void DropLoot()
    {
        // Check if lootItems is not null and has items
        if (lootItems.Length > 0)
        {
            print("loot items found"); // Debug message to indicate loot items are found
            // Randomly select a loot item from the array
            LootItem lootItemInstance = new LootItem();
            GameObject lootItem = Instantiate(lootItemInstance.FindItemByRandomWeight(lootItems), transform.position, Quaternion.identity);
            Collider[] colliders = lootItem.GetComponents<Collider>();
            Rigidbody rb = lootItem.GetComponent<Rigidbody>();
            rb.isKinematic = true;
        
            StartCoroutine(DelayedStuff(lootItem, rb, colliders));
        }
    }

    private float GetHeight(Collider[] colliders)
    {
        foreach (Collider collider in colliders)
        {
            if (collider.bounds.size.y > 0)
            {
                return collider.bounds.size.y;
            }
        }

        return 0f; // Return zero if no collider found
    }
    IEnumerator DelayedStuff(GameObject obj, Rigidbody rb, Collider[] colliders)
    {
        yield return new WaitForFixedUpdate();
        print("delayed stuff"); // Debug message to indicate the coroutine has started
        obj.transform.position = transform.position + GetHeight(colliders) / 2 * Vector3.up; // Adjust the position to be above the enemy
        rb.isKinematic = false; // Ensure the loot item is not kinematic
        rb.AddForce(new(UnityEngine.Random.Range(2f, 5f), 5f, UnityEngine.Random.Range(2f, 5f)), ForceMode.Impulse); // Add upward force to the loot item
        lootItems = null;
        print("enemy killed"); // Debug message to indicate the enemy is killed
        transform.gameObject.SetActive(false); // Deactivate the enemy object
    }
}

[Serializable]
public class LootItem
{
    [Tooltip("The prefab of the loot item. This should be a prefab that can be instantiated in the game world.")]
    public GameObject itemPrefab; // Prefab of the loot item
    [Tooltip("The weight of the item. Higher weight means more likely to drop.")]
    public int weight;
    [Tooltip("The range of the amount of items that can drop (min inclusive, max inclusive).")]
    public Vector2Int dropAmountRange;

    public int GetRandomDropAmount()
    {
        return UnityEngine.Random.Range(dropAmountRange.x, dropAmountRange.y + 1);
    }

    public int GetTotalWeight(LootItem[] lootItems)
    {
        int totalWeight = 0;
        foreach (LootItem item in lootItems)
        {
            totalWeight += item.weight;
        }
        return totalWeight;
    }

    public GameObject FindItemByWeight(LootItem[] lootItems, int weight)
    {
        foreach (LootItem item in lootItems)
        {
            if (weight < item.weight)
            {
                return item.itemPrefab;
            }
            weight -= item.weight;
        }
        return null; // Return null if no item found
    }
    public GameObject FindItemByRandomWeight(LootItem[] lootItems)
    {
        int totalWeight = GetTotalWeight(lootItems);
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        return FindItemByWeight(lootItems, randomWeight);
    }

    public float WeightToChance(int weight, int totalWeight)
    {
        return (float)weight / totalWeight;
    }
}
