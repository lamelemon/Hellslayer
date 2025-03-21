using UnityEngine;

public class Animation_player : MonoBehaviour
{
    Animator animator; // reference variable
    [SerializeField] PlayerController PlayerController;
    [SerializeField] PlayerInputManager GetInput;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Crouching and running
        if (PlayerController.IsCrouching)
        {
            animator.SetBool("is_crouching", true); // Set animation to crouching
        }

        else if (GetInput.SprintInput.IsPressed())
        {
            animator.SetBool("is_running", true); // Set animation to running
        }

        else
        {
            animator.SetBool("is_running", false); // Stop running animation
            animator.SetBool("is_crouching", false); // Set animation to standing
        }


        // walking
        if (GetInput.MoveValue.y > 0)
        {
            animator.SetBool("is_walking", true);
        }

        else if (GetInput.MoveValue.y < 0)
        {
            animator.SetBool("is_moving_backwards", true);
        }

        else
        {
            animator.SetBool("is_moving_backwards", false);
            animator.SetBool("is_walking", false);
        }


        // left right
        if (GetInput.MoveValue.x > 0)
        {
            animator.SetBool("is_moving_left", true);
        }

        else if (GetInput.MoveValue.x < 0)
        {
            animator.SetBool("is_moving_right", true);
        }

        else
        {
            animator.SetBool("is_moving_right", false);
            animator.SetBool("is_moving_left", false);
        }


        //jumping
        if (PlayerController.IsJumping)
        {
            animator.SetBool("is_jumping", true);
        }

        else
        {
            animator.SetBool("is_jumping", false);
        }
    }
}
