using UnityEngine;
using System.Collections.Generic;
public class AnimationChecker : MonoBehaviour
{
    private Animator animator;
    //Animator animator;
    private bool crouch_pressed = false;

    // making code shorter by naming the animation states. "what player is doing now"
    private Dictionary<string, string> ani = new Dictionary<string, string>
    {
        { "run", "running_player" },
        { "walk", "walking_player" },
        { "run_b", "running_backwards" },
        { "walk_b", "walking_backward" },
        { "jump", "jumping_player" },
        { "jump_b", "jump_backwards" },
        { "walk_r", "right_strafe_walk" }, // side movement animation satates  [
        { "walk_l", "left_strafe_walk" },
        { "run_r", "right_strafe_run" },
        { "run_l", "left_strafe_run" }, // ]
        { "crouch_i", "crouching_idle" }, // crouch animation states  [
        { "crouch_s", "Crouch_To_Stand" },
        { "crouch_w", "Crouch_Walk_Forward" },
        { "crouch_b", "Crouch_Walk_Back" },
        { "crouch_l", "Crouch_Walk_Left" },
        { "crouch_r", "Crouch_Walk_Right" } // ]
    };
    
    // making code shorter by naming the animation parameters. "What animation is playing now base of true or false bool parametres" 
    private Dictionary<string, string> pa = new Dictionary<string, string>
    {
        { "run", "is_running" },
        { "walk", "is_walking" },
        { "back", "is_moving_backwards" },
        { "jump", "is_jumping" },
        { "crouch", "is_crouching" },
        { "right", "is_moving_right" },
        { "left", "is_moving_left" }
    };

    void Start()
    {


        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject!");
        }
        if (audio_manager.Instance == null)
        {
            Debug.LogError("AudioManager instance is null!");
            return;  // Skip the rest of the logic if AudioManager is missing.
        }


    }



    void Update()
    {
        // || is or. This line check if player is has animation x or player is animation y or both
        // ! check correct key names and Dictionarys

        // WALK "all"
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(ani["walk"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["walk_b"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["walk_r"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["walk_l"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["crouch_w"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["crouch_b"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["crouch_l"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["crouch_r"])) 
        {
            if (animator.GetBool(pa["walk"]) || animator.GetBool(pa["back"]) || animator.GetBool(pa["right"]) || animator.GetBool(pa["left"]) || animator.GetBool(pa["crouch"]))
            {
                if (!audio_manager.Instance.sfxSource.isPlaying)
                {
                    audio_manager.Instance.PlaySFX("player_run", 0.75f); // calling audio with custom speed
                }
            }
        }

        // RUN "all"
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName(ani["run"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["run_b"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["run_r"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["run_l"]))
        {
            if (animator.GetBool(pa["run"]) || animator.GetBool(pa["back"]) || animator.GetBool(pa["right"]) || animator.GetBool(pa["left"]))
            {
                if (!audio_manager.Instance.sfxSource.isPlaying)
                {
                    audio_manager.Instance.PlaySFX("player_run", 1.0f);
                }
            }
        }

        // JUMP "-errors"
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName(ani["jump"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["jump_b"]))
        {
            if (animator.GetBool(pa["jump"]) || animator.GetBool(pa["back"]))
            {
                if (!audio_manager.Instance.sfxSource.isPlaying)
                {
                    audio_manager.Instance.PlaySFX("player_jump", 0.5f);
                }
            }
        }


        // CROUCH "stand up and to going crouch"
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName(ani["crouch_s"]) || animator.GetCurrentAnimatorStateInfo(0).IsName(ani["crouch_i"]))
        {
            if (animator.GetBool(pa["crouch"]))
            {
                if ((!audio_manager.Instance.sfxSource.isPlaying) && !crouch_pressed)
                {
                    audio_manager.Instance.PlaySFX("player_crouch", 1.0f);
                    crouch_pressed = true;
                }
            }
        }
        else
        {
            crouch_pressed = false;
        }


    }
}