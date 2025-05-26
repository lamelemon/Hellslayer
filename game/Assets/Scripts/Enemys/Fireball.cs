using UnityEngine;

public class BossFireball : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    public float lifetime = 5f;

    public void Launch(Vector3 dir, float spd)
    {
        direction = dir;
        speed = spd;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
