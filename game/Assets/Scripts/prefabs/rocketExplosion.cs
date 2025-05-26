using UnityEngine;

public class rocketExplosion : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, 2.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
