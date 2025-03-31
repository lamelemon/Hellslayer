using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class AnimationController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerInputManager getInput;
    public Animator animator;
    public Rigidbody rb;

    private bool wasJumpingLastFrame = false;

    void Update()
    {
        // Check if player is pressing any movement keys
        bool isMoving = getInput.MoveValue.y != 0 || getInput.MoveValue.x != 0;
        walk(isMoving);

        // Check if sprint button is pressed while moving
        if (isMoving && getInput.SprintInput.IsPressed()) 
        {
            run(true);
        }
        else
        {
            run(false);
        }

        // Check if the jump button is pressed (only set jump if it wasn't already set)
        if (playerController.IsJumping)
        {
            jump(true);
        }
        else if (!playerController.IsJumping && wasJumpingLastFrame)
        {
            jump(false);  // Reset jump state after the animation finishes
        }
        if (!playerController.IsOnFloor && !playerController.IsJumping)
        {
            fall(true);
        }
        else
        {
            fall(false);
        }

        wasJumpingLastFrame = playerController.IsJumping;  // Store the jump state for the next frame
    }

    void walk(bool isWalking)
    {
        animator.SetBool("is_walking", isWalking);
        //Debug.Log("is_walking set to: " + isWalking);
    }

    void run(bool isRunning)
    {
        animator.SetBool("is_running", isRunning);
        //Debug.Log("is_running set to: " + isRunning);
    }

    void jump(bool isJumping)
    {
        animator.SetBool("is_jumping", isJumping);
        //Debug.Log("is_jumping set to: " + isJumping);
    }

    void fall(bool is_falling)
    {
        animator.SetBool("is_falling", is_falling);
        //Debug.Log("is_falling set to: " + is_falling);
    }
}
