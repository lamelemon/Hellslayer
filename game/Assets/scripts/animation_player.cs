using UnityEngine;

public class animation_player : MonoBehaviour
{
    Animator animator; // reference variable
    public PlayerController PlayerController_script;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] PlayerInputSubscription GetInput;
    private PlayerController playerController;
    void Start()
    {
        animator = GetComponent<Animator>();
        GetInput = GetComponent<PlayerInputSubscription>();
        GameObject player = GameObject.Find("Player"); // Make sure this matches the actual GameObject name
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        bool isCrouching = false; // declare isCrouching here

        if (playerController != null)
        {
            // Access the isCrouching variable
            isCrouching = playerController.isCrouching;
        }
        if (isCrouching)
        {
            animator.SetBool("is_crouching", true); // Set animation to crouching
        }
        else
        {
            animator.SetBool("is_crouching", false); // Set animation to standing
        }


        // walking
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("is_walking", true);
        }

        if (!Input.GetKey(KeyCode.W))
        {
            animator.SetBool("is_walking", false);
        }


        // backwards moving
        if (Input.GetKey(KeyCode.S))
        {
            animator.SetBool("is_moving_backwards", true);
        }

        if (!Input.GetKey(KeyCode.S))
        {
            animator.SetBool("is_moving_backwards", false);
        }


        // right left
        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("is_moving_left", true);
        }
        //if (!Input.GetKey(KeyCode.A))
        else
        {
            animator.SetBool("is_moving_left", false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("is_moving_right", true);
        }
        else//if (!Input.GetKey(KeyCode.D))
        {
            animator.SetBool("is_moving_right", false);
        }


        // running
        if (GetInput.SprintInput)
        {
            animator.SetBool("is_running", true);
        }

        else
        {
            animator.SetBool("is_running", false);
        }


        //jumping
        if (GetInput.JumpInput)
        {
            animator.SetBool("is_jumping", true);
        }

        else
        {
            animator.SetBool("is_jumping", false);
        }
    }
}
