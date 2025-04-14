using UnityEngine;

public class Katana : MonoBehaviour
{
    public void RotateTo(Vector3 desiredRotation)
    {
        transform.rotation = Quaternion.Euler(desiredRotation);
    }

    private void Start()
    {
        // Rotate the Katana to (0, 0, 66.62) when the game starts
        //RotateTo(new Vector3(0, 0, 0f));

        // Offset the Katana's position to (0, 80, 0)
        //transform.position += new Vector3(0, 80, 0);
    }
}
