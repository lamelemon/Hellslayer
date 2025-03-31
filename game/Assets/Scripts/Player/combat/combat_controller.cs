using UnityEngine;

public class combat_controller : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] PlayerInputManager getInput;
    public Animator animator;
    


    void Update()
    {
        if (getInput.AttackInput.IsPressed())
        {
            Attack_Clicked(true);
        }
        else
        {
            Attack_Clicked(false);
        }
    }



        void Attack_Clicked(bool is_attacking)
    {
        animator.SetBool("is_attacking", is_attacking);
        //Debug.Log("is_falling set to: " + is_falling);
    }
}
