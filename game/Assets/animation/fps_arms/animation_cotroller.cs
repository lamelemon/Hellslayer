using UnityEngine;

public class animation_cotroller : MonoBehaviour
{
    [SerializeField] PlayerController PlayerController;
    [SerializeField] PlayerInputManager GetInput;
    Animator animator;
    private Rigidbody rb;
    //float velocity = 0.0f;

    void Awake()
    {
        // set reference
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // increases perfromance
        //VelocityHash = Animator.StringToHash("velocity")
    }

    void Update()
    { 
        if (GetInput.MoveValue.y > 0 )
        {
            bool forward_pressed = true;
        }
        bool sprint_pressed = GetInput.SprintInput.IsPressed();


        //Debug.Log("Velocity: " + velocity);
        //Debug.Log("Velocity: " + velocity.magnitude);
       // animator.SetFloat(rd.velocity, velocity)
    }
}
